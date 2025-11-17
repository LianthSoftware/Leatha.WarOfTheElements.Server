using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Leatha.WarOfTheElements.Common.Communication.Messages.Requests;
using Leatha.WarOfTheElements.Common.Communication.Messages.Responses;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.Objects.Validations;
using Leatha.WarOfTheElements.Server.Utilities;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OneOf;
using ValidationFailure = FluentValidation.Results.ValidationFailure;
using static BCrypt.Net.BCrypt;

namespace Leatha.WarOfTheElements.Server.Services
{
    public interface IAuthService
    {
        //Task<AuthLoginResult> AuthenticateAsync(string email, string password);

        Task<OneOf<LoginResponse, ValidationFailed>> CreatePlayerAsync(SignupRequest request);

        Task<OneOf<LoginResponse, ValidationFailed>> AuthenticateAsync(LoginRequest request);

        Task<bool> RevokeRefreshTokenAsync(Guid playerId, string? refreshToken);

        Task<OneOf<RefreshTokenResponse, ObjectNotFound>> RefreshTokenAsync(Guid playerId, string refreshToken);

        Task<ValidateTokenResponse> ValidateTokenAsync(string token);
    }

    public class AuthService : IAuthService
    {
        public AuthService(IMongoClient mongoClient, IConfiguration configuration)
        {
            _configuration = configuration;

            _mongoAuthDatabase = mongoClient.GetDatabase(Constants.MongoAuthDb);
            //_mongoGameDatabase = mongoClient.GetDatabase(Constants.MongoGameDb);
        }

        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _mongoAuthDatabase;
        //private readonly IMongoDatabase _mongoGameDatabase;

        public async Task<OneOf<LoginResponse, ValidationFailed>> CreatePlayerAsync(SignupRequest request)
        {
            //var validator = new CreateAccountRequestValidator();
            //var validationResult = await validator.ValidateAsync(request);
            //if (!validationResult.IsValid)
            //{
            //    // Bad Request.
            //    return new ValidationFailed(validationResult.Errors);
            //}

            var playerFilter = Builders<Player>.Filter.Eq(i => i.Email, request.Email.ToLower());
            //& Builders<Player>.Filter.Eq(i => i.ServerId, request.ServerId);

            var player = await _mongoAuthDatabase.GetCollection<Player>(nameof(Player))
                .Find(playerFilter)
                .SingleOrDefaultAsync();

            if (player != null)
            {
                // Not Found.
                return new ValidationFailed(new ValidationFailure(nameof(player), "Player already exists."));
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(request.Password, salt, true);

            player = new Player
            {
                PlayerId = Guid.NewGuid(),
                Email = request.Email,
                Created = DateTime.UtcNow,
                PasswordHash = hashedPassword,
                PlayerName = request.PlayerName,
            };

            await _mongoAuthDatabase.GetCollection<Player>(nameof(Player)).InsertOneAsync(player);

            var token = GenerateJwtToken(player);
            var refreshToken = await GenerateRefreshTokenAsync(player.PlayerId);

            var loginResponse = new LoginResponse
            {
                PlayerId = player.PlayerId,
                Email = player.Email,
                Created = player.Created,
                //PlayerStatus = account.PlayerStatus,
                AccessToken = token,
                RefreshToken = refreshToken?.Token
            };

            return loginResponse;
        }

        public async Task<OneOf<LoginResponse, ValidationFailed>> AuthenticateAsync(LoginRequest request)
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                // Bad Request.
                return new ValidationFailed(validationResult.Errors.Select(i => new ValidationFailure(i.PropertyName, i.ErrorMessage)).ToList());
            }

            var playerFilter = Builders<Player>.Filter.Eq(i => i.Email, request.Email.ToLower());
                               //& Builders<Player>.Filter.Eq(i => i.ServerId, request.ServerId);

            var player = await _mongoAuthDatabase.GetCollection<Player>(nameof(Player))
                .Find(playerFilter)
                .SingleOrDefaultAsync();

            if (player == null)
            {
                // Not Found.
                return new ValidationFailed(new ValidationFailure(nameof(player), "Player was not found."));
            }

            if (!Verify(request.Password, player.PasswordHash, true))
            {
                // Unauthorized.
                return new ValidationFailed(new ValidationFailure(nameof(request.Password), "Password is incorrect."));
            }

            var token = GenerateJwtToken(player);
            RefreshToken? refreshToken = null;
            if (request.RememberMe)
                refreshToken = await GenerateRefreshTokenAsync(player.PlayerId);

            var loginResponse = new LoginResponse
            {
                PlayerId = player.PlayerId,
                Email = player.Email,
                Created = player.Created,
                //PlayerStatus = account.PlayerStatus,
                AccessToken = token,
                RefreshToken = refreshToken?.Token
            };

            return loginResponse;
        }

        public async Task<bool> RevokeRefreshTokenAsync(Guid playerId, string? refreshToken)
        {
            // Search by default by player id.
            var filter = Builders<RefreshToken>.Filter.Eq(i => i.PlayerId, playerId);

            // If refresh token specified, add it to search.
            if (!String.IsNullOrWhiteSpace(refreshToken))
                filter &= Builders<RefreshToken>.Filter.Eq(i => i.Token, refreshToken);

            var collection = _mongoAuthDatabase.GetCollection<RefreshToken>(nameof(RefreshToken));
            var result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<OneOf<RefreshTokenResponse, ObjectNotFound>> RefreshTokenAsync(Guid playerId, string refreshToken)
        {
            var playerFilter = Builders<Player>.Filter.Eq(i => i.PlayerId, playerId);

            var player = await _mongoAuthDatabase.GetCollection<Player>(nameof(Player))
                .Find(playerFilter)
                .SingleOrDefaultAsync();

            if (player == null)
            {
                // Not Found.
                return new ObjectNotFound(nameof(player), $"Player (\"{playerId}\") was not found.");
            }

            var tokenFilter = Builders<RefreshToken>.Filter.Eq(i => i.PlayerId, playerId) &
                         Builders<RefreshToken>.Filter.Eq(i => i.Token, refreshToken);

            var token = await _mongoAuthDatabase.GetCollection<RefreshToken>(nameof(RefreshToken))
                .Find(tokenFilter)
                .SingleOrDefaultAsync();
            if (token == null)
            {
                // Not Found.
                return new ObjectNotFound(nameof(token), "Refresh token was not found.");
            }

            if (!token.IsActive)
            {
                return new ObjectNotFound(nameof(token), "Refresh token is expired."); // #TODO:Unauthorized
            }

            await RevokeRefreshTokenAsync(playerId, refreshToken);
            var newRefreshToken = await GenerateRefreshTokenAsync(playerId);
            var newAccessToken = GenerateJwtToken(player);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<ValidateTokenResponse> ValidateTokenAsync(string token)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // #TODO
                ValidateAudience = false, // #TODO
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key), // #TODO
                ClockSkew = TimeSpan.Zero // Adjust clock skew as needed
            };

            var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenValidationParameters);

            var playerId = Guid.Empty;
            if (result.IsValid)
            {
                var playerIdClaim = result.Claims["player_id"];
                if (String.IsNullOrWhiteSpace(playerIdClaim?.ToString()))
                    return new ValidateTokenResponse { IsTokenValid = false };

                playerId = Guid.Parse(playerIdClaim.ToString()!);
            }

            return new ValidateTokenResponse
            {
                PlayerId = playerId,
                IsTokenValid = result.IsValid
            };
        }

        private string GenerateJwtToken(Player player)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var claims = new List<Claim>
            {
                new Claim("player_id", player.PlayerId.ToString()),
                new Claim(ClaimTypes.Email, player.Email),
                new Claim(ClaimTypes.NameIdentifier, player.Email),
                new Claim(ClaimTypes.Name, player.Email),
                new Claim(ClaimTypes.Role, "Player") // #TODO
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(1), // #TODO: Config?
                Expires = DateTime.UtcNow.AddDays(1), // #TODO: Config?
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid? playerId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = new RefreshToken
            {
                PlayerId = playerId,
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddMonths(1), // #TODO: Config?
                Created = DateTime.UtcNow,
            };

            var collection = _mongoAuthDatabase.GetCollection<RefreshToken>(nameof(RefreshToken));
            await collection.InsertOneAsync(refreshToken);

            return refreshToken;
        }
    }
}

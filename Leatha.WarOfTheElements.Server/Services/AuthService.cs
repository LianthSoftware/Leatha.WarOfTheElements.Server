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
        Task<OneOf<LoginResponse, ValidationFailed>> CreateAccountAsync(SignupRequest request);

        Task<OneOf<LoginResponse, ValidationFailed>> AuthenticateAsync(LoginRequest request);

        Task<bool> RevokeRefreshTokenAsync(Guid accountId, string? refreshToken);

        Task<OneOf<RefreshTokenResponse, ObjectNotFound>> RefreshTokenAsync(Guid accountId, string refreshToken);

        Task<ValidateTokenResponse> ValidateTokenAsync(string token);
    }

    public sealed class AuthService : IAuthService
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

        public async Task<OneOf<LoginResponse, ValidationFailed>> CreateAccountAsync(SignupRequest request)
        {
            //var validator = new CreateAccountRequestValidator();
            //var validationResult = await validator.ValidateAsync(request);
            //if (!validationResult.IsValid)
            //{
            //    // Bad Request.
            //    return new ValidationFailed(validationResult.Errors);
            //}

            var accountFilter = Builders<Account>.Filter.Eq(i => i.Email, request.Email.ToLower());
            var account = await _mongoAuthDatabase.GetMongoCollection<Account>()
                .Find(accountFilter)
                .SingleOrDefaultAsync();

            if (account != null)
            {
                // Not Found.
                return new ValidationFailed(new ValidationFailure(nameof(account), "Account already exists."));
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(request.Password, salt, true);

            account = new Account
            {
                AccountId = Guid.NewGuid(),
                Email = request.Email,
                Created = DateTime.UtcNow,
                PasswordHash = hashedPassword,
            };

            await _mongoAuthDatabase.GetMongoCollection<Account>().InsertOneAsync(account);

            var token = GenerateJwtToken(account);
            var refreshToken = await GenerateRefreshTokenAsync(account.AccountId);

            var loginResponse = new LoginResponse
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Created = account.Created,
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

            var accountFilter = Builders<Account>.Filter.Eq(i => i.Email, request.Email.ToLower());
                               //& Builders<Player>.Filter.Eq(i => i.ServerId, request.ServerId);

            var account = await _mongoAuthDatabase.GetMongoCollection<Account>()
                .Find(accountFilter)
                .SingleOrDefaultAsync();

            if (account == null)
            {
                // Not Found.
                return new ValidationFailed(new ValidationFailure(nameof(account), "Account was not found."));
            }

            if (!Verify(request.Password, account.PasswordHash, true))
            {
                // Unauthorized.
                return new ValidationFailed(new ValidationFailure(nameof(request.Password), "Password is incorrect."));
            }

            var token = GenerateJwtToken(account);
            RefreshToken? refreshToken = null;
            if (request.RememberMe)
                refreshToken = await GenerateRefreshTokenAsync(account.AccountId);

            var loginResponse = new LoginResponse
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Created = account.Created,
                //PlayerStatus = account.PlayerStatus,
                AccessToken = token,
                RefreshToken = refreshToken?.Token
            };

            return loginResponse;
        }

        public async Task<bool> RevokeRefreshTokenAsync(Guid accountId, string? refreshToken)
        {
            // Search by default by player id.
            var filter = Builders<RefreshToken>.Filter.Eq(i => i.AccountId, accountId);

            // If refresh token specified, add it to search.
            if (!String.IsNullOrWhiteSpace(refreshToken))
                filter &= Builders<RefreshToken>.Filter.Eq(i => i.Token, refreshToken);

            var collection = _mongoAuthDatabase.GetCollection<RefreshToken>(nameof(RefreshToken));
            var result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<OneOf<RefreshTokenResponse, ObjectNotFound>> RefreshTokenAsync(Guid accountId, string refreshToken)
        {
            var accountFilter = Builders<Account>.Filter.Eq(i => i.AccountId, accountId);

            var account = await _mongoAuthDatabase.GetMongoCollection<Account>()
                .Find(accountFilter)
                .SingleOrDefaultAsync();

            if (account == null)
            {
                // Not Found.
                return new ObjectNotFound(nameof(account), $"Account (\"{ accountId }\") was not found.");
            }

            var tokenFilter = Builders<RefreshToken>.Filter.Eq(i => i.AccountId, accountId) &
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

            await RevokeRefreshTokenAsync(accountId, refreshToken);
            var newRefreshToken = await GenerateRefreshTokenAsync(accountId);
            var newAccessToken = GenerateJwtToken(account);

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

            var accountId = Guid.Empty;
            if (result.IsValid)
            {
                var accountIdClaim = result.Claims["account_id"];
                if (String.IsNullOrWhiteSpace(accountIdClaim?.ToString()))
                    return new ValidateTokenResponse { IsTokenValid = false };

                accountId = Guid.Parse(accountIdClaim.ToString()!);
            }

            return new ValidateTokenResponse
            {
                AccountId = accountId,
                IsTokenValid = result.IsValid
            };
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var claims = new List<Claim>
            {
                new Claim("account_id", account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.NameIdentifier, account.Email),
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.Role, "Player") // #TODO
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(1), // #TODO: Config?
                //Expires = DateTime.UtcNow.AddDays(1), // #TODO: Config?
                Expires = DateTime.UtcNow.AddMonths(1), // #TODO: Config?
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid accountId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = new RefreshToken
            {
                AccountId = accountId,
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddMonths(1), // #TODO: Config?
                Created = DateTime.UtcNow,
            };

            var collection = _mongoAuthDatabase.GetMongoCollection<RefreshToken>();
            await collection.InsertOneAsync(refreshToken);

            return refreshToken;
        }
    }
}

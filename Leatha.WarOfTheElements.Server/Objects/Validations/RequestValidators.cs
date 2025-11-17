using FluentValidation;
using Leatha.WarOfTheElements.Common.Communication.Messages.Requests;

namespace Leatha.WarOfTheElements.Server.Objects.Validations
{
    public static class RequestValidatorsConstants
    {
        public const int MinimumPasswordLength = 8;
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(i => i.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(RequestValidatorsConstants.MinimumPasswordLength).WithMessage($"Password must be at least {RequestValidatorsConstants.MinimumPasswordLength} characters long.");

            RuleFor(i => i.Email)
                .NotEmpty().WithMessage("E-mail must not be empty.")
                .EmailAddress().WithMessage("E-mail is invalid.");
        }
    }

    //public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    //{
    //    public CreateAccountRequestValidator()
    //    {
    //        RuleFor(i => i.Password)
    //            .NotEmpty().WithMessage("Password is required.")
    //            .MinimumLength(RequestValidatorsConstants.MinimumPasswordLength).WithMessage($"Password must be at least {RequestValidatorsConstants.MinimumPasswordLength} characters long.");

    //        RuleFor(i => i.Email)
    //            .NotEmpty().WithMessage("E-mail must not be empty.")
    //            .EmailAddress().WithMessage("E-mail is invalid.");
    //    }
    //}

    //public class ServerPlayerInfoValidator : AbstractValidator<ServerPlayerInfo>
    //{
    //    public ServerPlayerInfoValidator()
    //    {
    //        RuleFor(x => x.ServerId).GreaterThan(0).WithMessage("ServerId must be provided.");
    //        RuleFor(x => x.PlayerId).GreaterThan(0).WithMessage("PlayerId must be provided.");
    //    }
    //}

    //public class ServerAccountInfoValidator : AbstractValidator<ServerAccountInfo>
    //{
    //    public ServerAccountInfoValidator()
    //    {
    //        RuleFor(x => x.ServerId).GreaterThan(0).WithMessage("ServerId must be provided.");
    //        RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("AccountId must be provided.");
    //    }
    //}

    //public class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
    //{
    //    public CreateAssetRequestValidator()
    //    {
    //        RuleFor(x => x.Bytes).NotEmpty().WithMessage("Bytes content must be provided.");
    //        RuleFor(x => x.AssetName).NotEmpty().WithMessage("AssetName must be provided.");
    //        RuleFor(x => x.ContentType).NotEmpty().WithMessage("ContentType must be provided.");
    //    }
    //}

    //public class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
    //{
    //    public UpdateAssetRequestValidator()
    //    {
    //        RuleFor(x => x.AssetId).NotEmpty().WithMessage("AssetId must be provided.");
    //        RuleFor(x => x.Bytes).NotEmpty().WithMessage("Bytes content must be provided.");
    //        RuleFor(x => x.AssetName).NotEmpty().WithMessage("AssetName must be provided.");
    //        RuleFor(x => x.ContentType).NotEmpty().WithMessage("ContentType must be provided.");
    //    }
    //}
}

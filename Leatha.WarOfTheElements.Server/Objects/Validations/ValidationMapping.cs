using Leatha.WarOfTheElements.Common.Communication.Messages.Responses;

namespace Leatha.WarOfTheElements.Server.Objects.Validations
{
    public static class ValidationMapping
    {
        public static ValidationFailureResponse MapToResponse(this ValidationFailed validationFailed)
        {
            var errors = validationFailed.Errors.Select(i => new ValidationResponse
            {
                PropertyName = i.PropertyName,
                Message = i.ErrorMessage,
            });

            return new ValidationFailureResponse(errors);
        }
    }
}

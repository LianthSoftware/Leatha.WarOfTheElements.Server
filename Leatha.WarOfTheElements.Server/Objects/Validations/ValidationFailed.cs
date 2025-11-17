using FluentValidation.Results;

namespace Leatha.WarOfTheElements.Server.Objects.Validations
{
    public sealed class ValidationFailed
    {
        public ValidationFailed(IEnumerable<ValidationFailure> errors)
        {
            Errors = errors;
        }

        public ValidationFailed(ValidationFailure error) : this(new List<ValidationFailure> { error })
        {
        }

        public IEnumerable<ValidationFailure> Errors { get; set; }
    }
}

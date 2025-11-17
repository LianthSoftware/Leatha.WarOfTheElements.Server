namespace Leatha.WarOfTheElements.Common.Communication.Messages.Responses
{
    public class ValidationFailureResponse
    {
        public ValidationFailureResponse(IEnumerable<ValidationResponse> errors)
        {
            Errors = errors;
        }

        public IEnumerable<ValidationResponse> Errors { get; set; }
    }
}

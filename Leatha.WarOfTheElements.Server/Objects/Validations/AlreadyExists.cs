namespace Leatha.WarOfTheElements.Server.Objects.Validations
{
    public class AlreadyExists
    {
        public AlreadyExists()
        {
        }

        public AlreadyExists(string message)
        {
            Message = message;
        }

        public string? Message { get; set; }
    }
}

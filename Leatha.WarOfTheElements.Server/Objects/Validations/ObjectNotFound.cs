namespace Leatha.WarOfTheElements.Server.Objects.Validations
{
    public class ObjectNotFound
    {
        public ObjectNotFound()
        {
        }

        public ObjectNotFound(string propertyName, string message)
        {
            PropertyName = propertyName;
            Message = message;
        }

        public string? PropertyName { get; set; }

        public string? Message { get; set; }
    }
}

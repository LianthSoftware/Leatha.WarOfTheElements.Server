using Leatha.WarOfTheElements.Common.Communication.Messages.Responses;

namespace Leatha.WarOfTheElements.Common.Communication.Messages
{
    public class TransferMessage
    {
        public TransferMessage()
        {
            IsError = false;
        }

        public TransferMessage(string errorTitle, string errorMessage)
        {
            ErrorTitle = errorTitle;
            ErrorMessage = errorMessage;

            IsError = true;
        }

        public bool IsError { get; set; }

        public string? ErrorTitle { get; set; }

        public string? ErrorMessage { get; set; }


        public static TransferMessage CreateMessage()
        {
            return new TransferMessage();
        }

        public static TransferMessage CreateErrorMessage(string title, string description)
        {
            return new TransferMessage(title, description);
        }

        public static TransferMessage<TMessage> CreateMessage<TMessage>(TMessage data)
        {
            return new TransferMessage<TMessage>(data);
        }

        public static TransferMessage<TMessage> CreateErrorMessage<TMessage>(string title, string description)
        {
            return new TransferMessage<TMessage>(title, description);
        }
    }

    public class TransferMessage<TMessageData> : TransferMessage// where TMessageData : class
    {
        public TransferMessage()
        {
            IsError = false;
        }

        public TransferMessage(TMessageData data)
        {
            Data = data;
            IsError = false;
        }

        public TransferMessage(string errorTitle, string errorMessage)// : base(errorTitle, errorMessage)
        {
            ErrorTitle = errorTitle;
            ErrorMessage = errorMessage;
            Data = default;
            IsError = true;
        }

        public TransferMessage(string errorTitle, string errorMessage, TMessageData data)// : base(errorTitle, errorMessage)
        {
            ErrorTitle = errorTitle;
            ErrorMessage = errorMessage;
            Data = data;
            IsError = true;
        }

        public TMessageData? Data { get; set; }// = null!;
    }

    //public sealed class ValidationFailureTransferMessage<TMessageData> : TransferMessage<ValidationFailureResponse>
    //{
    //    public ValidationFailureTransferMessage(IEnumerable<ValidationResponse> errors)
    //    {
    //        Errors = errors.ToList();
    //    }

    //    public IReadOnlyList<ValidationResponse> Errors { get; }

    //    public static ValidationFailureTransferMessage<TMessageData> CreateValidationErrorMessage(ValidationResponse error)
    //    {
    //        return new ValidationFailureTransferMessage<TMessageData>([ error ]);
    //    }

    //    public static ValidationFailureTransferMessage<TMessageData> CreateValidationErrorMessage(IEnumerable<ValidationResponse> errors)
    //    {
    //        return new ValidationFailureTransferMessage<TMessageData>(errors);
    //    }
    //}
}

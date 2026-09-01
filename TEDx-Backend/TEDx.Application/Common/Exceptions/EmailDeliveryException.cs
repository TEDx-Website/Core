namespace TEDx.Application.Common.Exceptions;

// Thrown when a transactional email could not be handed to the mail server.
public sealed class EmailDeliveryException : Exception
{
    public EmailDeliveryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

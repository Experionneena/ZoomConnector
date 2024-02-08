namespace Bayada.Joy.WorkDayConnector.CustomException
{
    public class ZoomConnectorException : Exception
    {
        public ZoomConnectorException()
        { }

        public ZoomConnectorException(string message) : base(message)
        { }

        public ZoomConnectorException(string message, Exception innerException) : base(message, innerException)
        { }

    }
}

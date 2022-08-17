using System.Runtime.Serialization;

namespace StockQuoteAlert
{
    [Serializable]
    internal class InvalidAPICall : Exception
    {
        public InvalidAPICall()
        {
        }

        public InvalidAPICall(string? message) : base(message)
        {
        }

        public InvalidAPICall(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidAPICall(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
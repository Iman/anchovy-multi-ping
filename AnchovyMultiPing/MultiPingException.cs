using System;

namespace AnchovyMultiPing
{
    internal class MultiPingException : Exception
    {
        public MultiPingException()
            : base() {}

        public MultiPingException(string message)
            : base(message) {}

        public MultiPingException(string format, params object[] args)
            : base(string.Format(format, args)) {}

        public MultiPingException(string message, Exception innerException)
            : base(message, innerException) {}

        public MultiPingException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) {}
    }
}

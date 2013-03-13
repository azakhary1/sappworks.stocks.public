
namespace Stocks.Common
{
    using System;

    [Serializable]
    public class OAuthGetRequestTokenException : AuthenticationException
    {
        public OAuthGetRequestTokenException() { }
        public OAuthGetRequestTokenException(string message) : base(message) { }
        public OAuthGetRequestTokenException(string message, Exception inner) : base(message, inner) { }
        protected OAuthGetRequestTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

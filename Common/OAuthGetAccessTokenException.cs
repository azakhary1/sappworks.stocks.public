
namespace Stocks.Common
{
    using System;

    [Serializable]
    public class OAuthGetAccessTokenException : AuthenticationException
    {
        public OAuthGetAccessTokenException() { }
        public OAuthGetAccessTokenException(string message) : base(message) { }
        public OAuthGetAccessTokenException(string message, Exception inner) : base(message, inner) { }
        protected OAuthGetAccessTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

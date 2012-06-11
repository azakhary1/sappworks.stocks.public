using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    [Serializable]
    public class OAuthGetAccessTokenException : OAuthException
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

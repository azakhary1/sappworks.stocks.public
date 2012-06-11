using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    [Serializable]
    public class OAuthGetRequestTokenException : OAuthException
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

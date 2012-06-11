using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Framework;

namespace Stocks.ServiceClients.ETrade
{
    public class OAuthToken : IToken
    {
        public string SessionHandle
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string TokenSecret
        {
            get;
            set;
        }

        public string ConsumerKey
        {
            get;
            set;
        }

        public string Realm
        {
            get;
            set;
        }
    }
}

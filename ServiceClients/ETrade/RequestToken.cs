using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Framework;

namespace Stocks.ServiceClients.ETrade
{
    /// <summary>
    /// This is the request token.
    /// </summary>
    /// 
    public class RequestToken : OAuthToken
    {
        public DateTime Expires { get; set; }

        public bool Expired
        {
            get { return (Expires >= DateTime.Now); }
        }
    }
}

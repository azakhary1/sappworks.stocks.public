
namespace Stocks.ServiceClients.ETrade
{
    using System;

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

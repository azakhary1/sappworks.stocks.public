
namespace Stocks.ServiceClients.ETrade
{
    using DevDefined.OAuth.Framework;

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

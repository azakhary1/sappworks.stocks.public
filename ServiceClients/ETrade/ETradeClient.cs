
namespace Stocks.ServiceClients.ETrade
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;

    using DevDefined.OAuth.Consumer;
    using DevDefined.OAuth.Framework;

    using Stocks.Common;
    using Stocks.ServiceClients.ETrade.ObjectModel;

    public class ETradeClient
    {
        private const string
            RequestUrl = "https://etws.etrade.com/oauth/request_token",
            AuthorizeUrl = "https://us.etrade.com/e/t/etws/authorize",
            AccessUrl = "https://etws.etrade.com/oauth/access_token",
            DataUrl = "https://etws.etrade.com",
            SandboxDataUrl = "https://etwssandbox.etrade.com";

        private readonly OAuthSession _session;
        private readonly OAuthConsumerContext _consumerContext;
        private RequestToken _requestToken;
        private readonly ConsumerToken _consumerToken;
        private readonly bool _productionMode;

        private AccessToken _accessToken;
        public AccessToken AccessToken 
        { 
            get { return _accessToken; } 
            set { _accessToken = value; }
        }

        // Rate Limits:
        // Orders          2 incoming requests per second per user
        // Accounts        2 incoming requests per second per user
        // Quotes          4 incoming requests per second per user
        private readonly static TokenBucket
            _ordersTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 30)),
            _accountsTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 28)),
            _quotesTokenBucket = new TokenBucket(4, new TimeSpan(0, 0, 0, 1, 30));

        public ETradeClient(ConsumerToken consumerToken, AccessToken accessToken = null, bool productionMode = false)
        {
            _consumerToken = consumerToken;

            _consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerToken.Token,
                ConsumerSecret = consumerToken.TokenSecret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true,
                CallBack = "oob"
            };

            _session = new OAuthSession(_consumerContext, RequestUrl, AuthorizeUrl, AccessUrl);

            _productionMode = productionMode;
            _accessToken = accessToken;
        }

        private RequestToken GetRequestToken()
        {
            if (!_consumerToken.IsSet()) { throw new OAuthGetRequestTokenException("Consumer token and secret are required."); }
            if (_consumerContext == null) { throw new OAuthGetRequestTokenException("Consumer context is not set up."); }
            if (_session == null) { throw new OAuthGetRequestTokenException("OAuthSession is not estabblished"); }

            _requestToken = _session.GetRequestToken().ToRequestToken();

            if (_requestToken == null)
            {
                throw new OAuthGetRequestTokenException("Unable to get Request token.");
            }

            return _requestToken;
        }

        public string GetUserAuthorizationUrlForToken()
        {
            if (_requestToken == null)
            {
                _requestToken = GetRequestToken();
            }

            return _session.GetUserAuthorizationUrlForToken(_consumerToken.Token, _requestToken);
        }

        public AccessToken GetAccessToken(string verificationKey)
        {
            if (!_consumerToken.IsSet()) { throw new OAuthGetAccessTokenException("Consumer token and secret are required."); }
            if (!_requestToken.IsSet()) { throw new OAuthGetAccessTokenException("Request token not set, you need to try getting the verification key again."); }
            if (_requestToken.Expired) { throw new OAuthGetAccessTokenException("Request token has expired, you need to try getting the verification key again."); }
            if (string.IsNullOrWhiteSpace(verificationKey)) { throw new OAuthGetAccessTokenException("Verification key is required you need to get it from etrade first."); }
            if (_session == null) { throw new OAuthGetRequestTokenException("OAuthSession is not estabblished"); }

            _accessToken = _session.ExchangeRequestTokenForAccessToken(_requestToken, verificationKey).ToAccessToken();

            if (_accessToken == null)
            {
                throw new OAuthGetAccessTokenException("Unable to get Request token.");
            }

            return _accessToken;
        }

        private static void ObeyRequestRateLimits(Type t)
        {
            if (t is IBelongToAccountService)
            {
                // get from accounts bucket
                _accountsTokenBucket.Consume();
            }
            else if (t is IBelongToMarketService)
            {
                // get from market bucket
                _quotesTokenBucket.Consume();
            }
            else if (t is IBelongToOrderService)
            {
                // get from order bucket
                _ordersTokenBucket.Consume();
            }
        }

        public T Get<T>(object queryData = null)
            where T : IResource, new()
        {
            var resourceType = typeof(T);

            ObeyRequestRateLimits(resourceType);

            string url = GetUrl<T>(queryData);

            var serializer = new XmlSerializer(resourceType);

            try
            {
                using (var responseStream = _session.Request(_accessToken).Get().ForUrl(url).ToWebResponse().GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    if (responseStream != null) responseStream.CopyTo(memoryStream);

                    memoryStream.Position = 0;

                    try
                    {
                        return (T)serializer.Deserialize(memoryStream);
                    }
                    catch (InvalidOperationException ex)
                    {
                        memoryStream.Position = 0;

                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            throw new DeserializeException(
                                ex.Message,
                                streamReader.ReadToEnd(),
                                ex
                            );
                        }
                    }
                }
            }
            catch(OAuthException ex)
            {
                throw new AuthenticationException(ex.InnerException.Message, ex);
            }
        }

        public TResult Post<TRequest, TResult>(TRequest request)
            where TRequest : IResource, IRequest, new() 
        {
            var resourceType = typeof(TResult);

            ObeyRequestRateLimits(resourceType);

            string url = GetUrl<TRequest>();

            var serializer = new XmlSerializer(resourceType);

            var requestDesc = _session.Request(_accessToken).Post().ForUrl(url).GetRequestDescription();
            
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = requestDesc.Method;

            foreach (string h in requestDesc.Headers.Keys)
            {
                webRequest.Headers.Set(h, requestDesc.Headers[h]);
            }

            webRequest.ContentType = "application/xml";
            var bytes = Encoding.UTF8.GetBytes(request.ToXml());
            webRequest.ContentLength = bytes.Length;

            using(Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(bytes, 0, bytes.Length);
            }

            using (var webResponse = webRequest.GetResponse())
            using (var responseStream = webResponse.GetResponseStream())
            {
                return (TResult)serializer.Deserialize(responseStream);
            }
        }

        private string GetUrl<T>(object queryData = null)
            where T : IResource, new()
        {
            string resourceName = new T().GetResourceName(_productionMode);

            return (_productionMode ? DataUrl : SandboxDataUrl) + (queryData != null ? resourceName.Inject(queryData) : resourceName);
        }
    }
}

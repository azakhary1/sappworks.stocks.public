using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

using Stocks.ServiceClients.ETrade.ObjectModel;
using Stocks.Common;
using System.Collections.Specialized;

namespace Stocks.ServiceClients.ETrade
{
    public class ETradeClient
    {
        private const string
            REQUEST_URL = "https://etws.etrade.com/oauth/request_token",
            AUTHORIZE_URL = "https://us.etrade.com/e/t/etws/authorize",
            ACCESS_URL = "https://etws.etrade.com/oauth/access_token",
            DataUrl = "https://etws.etrade.com",
            SandboxDataUrl = "https://etwssandbox.etrade.com";

        private OAuthSession _session;
        private OAuthConsumerContext _consumerContext;
        private RequestToken _requestToken;
        private ConsumerToken _consumerToken;
        private bool _productionMode;

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
        // Notifications   2 incoming requests per second (per user?)
        private static TokenBucket
            _ordersTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 150)),
            _accountsTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 150)),
            _quotesTokenBucket = new TokenBucket(4, new TimeSpan(0, 0, 0, 1, 150)),
            _notificationsTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 150));

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

            _session = new OAuthSession(_consumerContext, REQUEST_URL, AUTHORIZE_URL, ACCESS_URL);

            _productionMode = productionMode;
            _accessToken = accessToken;
        }

        private RequestToken GetRequestToken()
        {
            if (!_consumerToken.IsSet()) { throw new OAuthGetRequestTokenException("Consumer token and secret are required."); }
            if (_consumerContext == null) { throw new OAuthGetRequestTokenException("Consumer context is not set up."); }
            if (_session == null) { throw new OAuthGetRequestTokenException("OAuthSession is not estabblished"); }

            var _requestToken = _session.GetRequestToken().ToRequestToken();

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

        public string GetAccountBalanceAsString(int accountId)
        {
            string url = GetUrl<AccountBalanceResponse>(new { accountId = accountId });

            return _session.Request(_accessToken).Get().ForUrl(url).ToString();
        }

        public string GetOrders(int accountId)
        {
            string url = GetUrl<GetOrderListResponse>(new { accountId = accountId });

            return _session.Request(_accessToken).Get().ForUrl(url).ToString();
        }

        private void ObeyRequestRateLimits(string url)
        {
            if (url.Contains("/accounts"))
            {
                // get from accounts bucket
                _accountsTokenBucket.Consume();
            }
            else if (url.Contains("/market"))
            {
                // get from market bucket
                _quotesTokenBucket.Consume();
            }
            else if (url.Contains("/order"))
            {
                // get from order bucket
                _ordersTokenBucket.Consume();
            }
            else if (url.Contains("/notification"))
            {
                // get from notification bucket
                _notificationsTokenBucket.Consume();
            }
        }

        public T Get<T>(object queryData = null)
            where T : IResource, new()
        {
            string url = GetUrl<T>(queryData);

            ObeyRequestRateLimits(url);

            var serializer = new XmlSerializer(typeof(T));

            try
            {
                using (var responseStream = _session.Request(_accessToken).Get().ForUrl(url).ToWebResponse().GetResponseStream())
                {
                    try
                    {
                        return (T)serializer.Deserialize(responseStream);
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.InnerException is XmlException)
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                throw new XmlFormatException(
                                    ex.InnerException.Message,
                                    streamReader.ReadToEnd(),
                                    ex.InnerException
                                );
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch(DevDefined.OAuth.Framework.OAuthException ex)
            {
                throw new Stocks.Common.OAuthException(ex.InnerException.Message, ex);
            }
        }

        public TResult Post<TRequest, TResult>(TRequest request)
            where TRequest : IResource, IRequest, new() 
        {
            string url = GetUrl<TRequest>();

            ObeyRequestRateLimits(url);
            
            //var requestBody = request.ToXml();

            var serializer = new XmlSerializer(typeof(TResult));

            var requestDesc = _session.Request(_accessToken).Post().ForUrl(url).GetRequestDescription();
            
            var hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.Method = requestDesc.Method;

            foreach (string h in requestDesc.Headers.Keys)
            {
                hwr.Headers.Set(h, requestDesc.Headers[h]);
            }

            hwr.ContentType = "application/xml";
            var bytes = Encoding.UTF8.GetBytes(request.ToXml());
            hwr.ContentLength = bytes.Length;

            using(Stream dataStream = hwr.GetRequestStream())
            {
                dataStream.Write (bytes, 0, bytes.Length);
            }

            using (var webResponse = hwr.GetResponse())
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

        public XDocument GetWebResponseAsXml(HttpWebResponse response)
        {
            XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
            XDocument xdoc = XDocument.Load(xmlReader);
            xmlReader.Close();
            return xdoc;
        }

        public string GetWebResponseAsString(HttpWebResponse response)
        {
            Encoding enc = System.Text.Encoding.GetEncoding(1252);
            StreamReader loResponseStream = new
            StreamReader(response.GetResponseStream(), enc);
            return loResponseStream.ReadToEnd();
        }
    }
}

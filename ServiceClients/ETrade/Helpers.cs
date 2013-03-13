
namespace Stocks.ServiceClients.ETrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using DevDefined.OAuth.Framework;

    public static class Helpers
    {
        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(HttpUtility.UrlDecode(value));
        }

        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        public static string ToDelimitedString(this IEnumerable<string> value, string delimiter)
        {
            return string.Join(delimiter, from v in value select v);
        }
    }

    public static class OAuthTokenExtensions
    {
        public static bool IsSet(this IToken token)
        {
            return (token != null && !string.IsNullOrWhiteSpace(token.Token) && !string.IsNullOrWhiteSpace(token.TokenSecret));
        }

        public static RequestToken ToRequestToken(this IToken requestToken)
        {
            return 
                requestToken == null ? null :
                new RequestToken
                {
                    ConsumerKey = requestToken.ConsumerKey,
                    Expires = DateTime.Now,
                    Realm = requestToken.Realm,
                    SessionHandle = requestToken.SessionHandle,
                    Token = requestToken.Token,
                    TokenSecret = requestToken.TokenSecret
                };
        }

        public static AccessToken ToAccessToken(this IToken accessToken)
        {
            return
                accessToken == null ? null :
                new AccessToken
                {
                    ConsumerKey = accessToken.ConsumerKey,
                    Realm = accessToken.Realm,
                    SessionHandle = accessToken.SessionHandle,
                    Token = accessToken.Token,
                    TokenSecret = accessToken.TokenSecret
                };
        }
    }
}

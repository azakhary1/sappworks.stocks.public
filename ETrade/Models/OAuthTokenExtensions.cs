
namespace Sappworks.Stocks.ETrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using DevDefined.OAuth.Framework;
    using System.Globalization;

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
                    Expires = DateTime.Now.AddMinutes(4).AddSeconds(45),
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

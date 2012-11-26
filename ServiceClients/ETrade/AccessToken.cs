using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Framework;

namespace Stocks.ServiceClients.ETrade
{
    /// <summary>
    /// This is the OAuth access token granted after the user authenticates and authorizes.
    /// </summary>
    public class AccessToken : OAuthToken
    {
    }

    public static class AccessTokenExtensions
    {
        public static Stocks.Common.OAuthToken ToAccessToken(this AccessToken value)
        {
            return
                new Stocks.Common.OAuthToken
                {
                    Token = value.Token,
                    Secret = value.TokenSecret
                };
        }
    }
}

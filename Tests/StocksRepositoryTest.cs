
namespace Sappworks.Stocks.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Sappworks.Stocks;
    using Sappworks.Stocks.ETrade;
    using System.Collections.Concurrent;

    /// <summary>
    ///This is a test class for ETradeServiceClientTest and is intended
    ///to contain all ETradeServiceClientTest Unit Tests
    ///</summary>
    [TestClass]
    public class ETradeClientTest
    {
        private Stocks.OAuthToken _consumerToken;
        private Stocks.OAuthToken _accessToken;

        [TestInitialize]
        public void MyTestInitialize()
        {
            // setup sandbox consumer token
            //_consumerToken = new OAuthToken
            //{
            //    Token = "",
            //    Secret = ""
            //};

            // prod consumer token
            _consumerToken = new Stocks.OAuthToken
            {
                Token = "",
                Secret = ""
            };

            _accessToken = new Stocks.OAuthToken
            {
                Token = "",
                Secret = ""
            };
        }

        /// <summary>
        ///A test for GetRequestToken
        ///</summary>
        [TestMethod]
        public void GetOAuthUserActionUriTest()
        {
            var client = new ETradeClient(_consumerToken);

            var userActionUri = client.GetUserAuthorizationUrl();

            Assert.IsNotNull(userActionUri);
        }

        /// <summary>
        ///A test for GetAccounts
        ///</summary>
        [TestMethod]
        public void ParallelTokenBucketTest()
        {
            var client = new ETradeClient(_consumerToken);

            var tokens =
                new[] { 
                    new { client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client, consumerToken = _consumerToken, accessToken = _accessToken }
                };

            var accounts = new ConcurrentBag<Account>();

            var sw = new Stopwatch();
            sw.Start();

            Parallel.ForEach(
                tokens,
                t => accounts.Add(t.client.GetAccount())
            );

            sw.Stop();

            Assert.IsNotNull(accounts);
        }

        /// <summary>
        ///A test for ExecuteOrders
        ///</summary>
        [TestMethod]
        public void ExecuteOrdersTest()
        {
            var client = new ETradeClient(_consumerToken);

            const int accountId = 30049872;

            var orders =
                new List<Order>
                {
                    new Order
                    {
                        IsSale = false,
                        Price = 1d,
                        Quantity = 1,
                        Symbol = "IBM"
                    },
                    new Order
                    {
                        IsSale = true,
                        Price = 1d,
                        Quantity = 1,
                        Symbol = "MSFT"
                    }
                };

            var response = client.ExecuteOrders(accountId, orders);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Any());
        }
    }
}

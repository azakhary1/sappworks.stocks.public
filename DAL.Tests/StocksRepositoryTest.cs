using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Stocks.Common;
using Stocks.DAL;
using Stocks.ServiceClients.ETrade.ObjectModel;

namespace Stocks.Tests
{
    /// <summary>
    ///This is a test class for ETradeServiceClientTest and is intended
    ///to contain all ETradeServiceClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ETradeClientTest
    {
        private OAuthToken _consumerToken;
        private OAuthToken _accessToken;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {


        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // setup sandbox consumer token
            _consumerToken = new OAuthToken
            {
                Token = "",
                Secret = ""
            };

            //// prod consumer token
            //_consumerToken = new OAuthToken
            //{
            //    Token = "",
            //    Secret = ""
            //};

            _accessToken = new OAuthToken
            {
                Token = "",
                Secret = ""
            };
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        /// <summary>
        ///A test for GetRequestToken
        ///</summary>
        [TestMethod()]
        public void GetOAuthUserActionUriTest()
        {
            var client = new StocksRepository(_consumerToken);

            var userActionUri = client.GetUserAuthorizationUrl();

            Assert.IsNotNull(userActionUri);
        }

        /// <summary>
        ///A test for GetAccounts
        ///</summary>
        [TestMethod()]
        public void GetAccountsTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var client = new StocksRepository(_consumerToken, _accessToken, true);

            var accounts = client.GetAccounts();

            sw.Stop();

            Assert.IsNotNull(accounts);
        }

        /// <summary>
        ///A test for GetAccounts
        ///</summary>
        [TestMethod()]
        public void ParallelTokenBucketTest()
        {
            var client = new StocksRepository(_consumerToken, _accessToken);

            var tokens =
                new[] { 
                    new { client = client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client = client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client = client, consumerToken = _consumerToken, accessToken = _accessToken },
                    new { client = client, consumerToken = _consumerToken, accessToken = _accessToken }
                };

            List<Account> accounts = new List<Account>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

                Parallel.ForEach(
                    tokens,
                    t =>
                    {
                        accounts.AddRange(t.client.GetAccounts());
                    }
                );

            sw.Stop();

            Assert.IsNotNull(accounts);
        }


        /// <summary>
        ///A test for GetAccountById
        ///</summary>
        [TestMethod()]
        public void GetAccountByIdTest()
        {
            var client = new StocksRepository(_consumerToken, _accessToken);
            
            // one of the sandbox accounts
            int accountId = 30049872;
            var actual = client.GetAccountById(accountId);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetAccountById
        ///</summary>
        [TestMethod()]
        public void GetOrdersStringTest()
        {
            var client = new Stocks.ServiceClients.ETrade.ETradeClient(
                new ServiceClients.ETrade.ConsumerToken { Token = _consumerToken.Token, TokenSecret = _consumerToken.Secret },
                new ServiceClients.ETrade.AccessToken { Token = _accessToken.Token, TokenSecret = _accessToken.Secret}
            );

            // one of the sandbox accounts
            int accountId = 30049872;
            var actual = client.GetOrders(accountId);

            Assert.IsNotNull(actual);
        }

        ///// <summary>
        /////A test for GetAccountById
        /////</summary>
        //[TestMethod()]
        //public void GetQuoteStringTest()
        //{
        //    var client = new Stocks.ServiceClients.ETrade.ETradeClient(
        //        new ServiceClients.ETrade.ConsumerToken { Token = _consumerToken.Token, TokenSecret = _consumerToken.Secret },
        //        new ServiceClients.ETrade.AccessToken { Token = _accessToken.Token, TokenSecret = _accessToken.Secret },
        //        true
        //    );

        //    var actual = client.GetQuoteString();

        //    Assert.IsNotNull(actual);
        //}

        /// <summary>
        ///A test for ExecuteOrders
        ///</summary>
        [TestMethod()]
        public void ExecuteOrdersTest()
        {
            var client = new StocksRepository(_consumerToken, _accessToken);

            // one of the sandbox accounts
            int accountId = 30049872;

            var orders = 
                new List<Order>
                {
                    new Order
                    {
                        IsSale = false,
                        Price = 1m,
                        Quantity = 1,
                        Symbol = "HP"
                    },
                    new Order
                    {
                        IsSale = true,
                        Price = 1m,
                        Quantity = 1,
                        Symbol = "MSFT"
                    }
                }; 

            var response = client.ExecuteOrders(accountId, orders);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Count() > 0);

        }
    }
}


namespace Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    using Sappworks.Stocks;
    using Sappworks.Stocks.ETrade;

    class Program
    {
        static OAuthStockbrokerServiceInterface stockBrokerService;
        
        // this token identifies your program
        static Sappworks.Stocks.OAuthToken SandboxConsumerToken = 
            new Sappworks.Stocks.OAuthToken
            {
                Token = "2554ae24516c98c9ee78489dc18c46c5",
                Secret = "d76b6b6b487dfc7c01186540b4a94852"
            };

        static Sappworks.Stocks.OAuthToken accessToken;

        static void Main(string[] args)
        {
            stockBrokerService = new ETradeClient(SandboxConsumerToken, false, accessToken);

            OAuthProcess();

            // todo:
            //- get account
            //- get quotes
            //- get position quotes
            //- get open order symbols
            //- execute limit orders

            ExitPrompt();
        }

        static void Complain(AuthenticationException ex)
        {
            Console.WriteLine(ex.RequestUri);
            Console.WriteLine();
            Console.WriteLine("Request Headers:" + Environment.NewLine);
            Console.WriteLine(string.Join(Environment.NewLine + "\t", ex.RequestHeaders));
            Console.WriteLine();
            Console.WriteLine("Problem: " + ex.Problem);
            Console.WriteLine();
            Console.WriteLine("Problem advice: " + ex.ProblemAdvice);
            Console.WriteLine();
            Console.WriteLine("Parameters absent: " + string.Join(", ", ex.ParametersAbsent));
            Console.WriteLine();
            Console.WriteLine("Parameters rejected: " + string.Join(", ", ex.ParametersRejected));
            
            ExitPrompt(true);
        }

        static void ExitPrompt(bool error = false)
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Environment.Exit(error ? 1 : 0);
        }

        static void OAuthProcess()
        {
            // the goal is to obtain the access token
            // the access token authenticates every request
            // it will expire after 2 hours idle and 12am pacific time 

            if (!accessToken.IsSet())
            {
                // have the user log in and return to your app with the verification key
                try
                {
                    Process.Start(stockBrokerService.GetUserAuthorizationUrl());
                }
                catch (OAuthGetRequestTokenException ex)
                {
                    Complain(ex);
                }
            }

            string verificationKey;

            do
            {
                Console.Write("Enter verification key: ");

                verificationKey = Console.ReadLine();
            }
            while (verificationKey != null);

            try
            {
                // now that we have the verification key, attempt to exchange it for the access token
                accessToken = stockBrokerService.GetAccessToken(verificationKey);
            }
            catch (OAuthGetAccessTokenException ex)
            {
                Complain(ex);
            }
        }
    }
}

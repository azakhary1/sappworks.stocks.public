#The Missing ETrade C\# REST Client

This is a not-quite-feature-complete fully managed client for the ETrade API.  It does only a fraction of what the API offers, namely:
 - Accounts & Balances
 - Market Data
 - Basic Order Execution

This set of libraries is used in production in my trading automation currently, so the features that are implemented work at least, though not all of them it be designed optimally.  Many of the shortcomings are due to the current state of the ETrade documentation (it's bad).  So in the interest of reducing frustration globally, I offer these humble libraries.

##Examples
```csharp
  var client = new StocksRepository(_consumerToken, _accessToken);
  
  // get all accounts
  var accounts = client.GetAccounts();
```

```csharp
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

  // execute some orders
  var response = client.ExecuteOrders(accountId, orders);
```

##Special Thanks
**The OAuth Bits**  
To handle OAuth we're using the [DevDefined ETrade Oauth branch](https://github.com/jejernig/DevDefined.OAuth---Etrade) - thanks to [jejernig](http://stackoverflow.com/users/616499/jejernig) for that!

**The Missing Market Data (thanks Yahoo)**  
After getting basic OAuth working I realized there was no way (at least no documented way) to get a moving average.  So a bit of code was ported from the [Jarloo YahooStockEngine](http://www.jarloo.com/get-near-real-time-stock-data-from-yahoo-finance/) - thanks to Kelly Elias for that!

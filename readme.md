#The Missing C# E*Trade Client

This is a not-quite-feature-complete fully managed client for the ETrade API.  It does only a fraction of what the API offers, namely:
 - Accounts & Balances
 - Market Data
 - Basic Order Execution (just limit orders right now)

This set of libraries is used in production in my trading automation, so the implementations do work, though not all of them may be designed well...  

##Example
```csharp
  // create the service client
  var client = new StocksRepository(_consumerToken, _accessToken);
  
  // get all accounts
  var accounts = client.GetAccounts();
```

```csharp
  int accountId = 30049872;
  
  // create some orders
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

  // execute them
  var response = client.ExecuteOrders(accountId, orders);
```

See this page to get started:  [Getting Started](https://github.com/bmsapp/sappworks.stocks.public/wiki/Getting-Started)

##Special Thanks
**The OAuth Bits**  
To [jejernig](http://stackoverflow.com/users/616499/jejernig) for [DevDefined.OAuth.Etrade](https://github.com/jejernig/DevDefined.OAuth---Etrade/network).  Excellent work!

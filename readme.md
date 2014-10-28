#The Missing C# E*Trade Client

This is a not-quite-feature-complete fully managed client for the ETrade API.  So far it can:
 - Get accounts & balances
 - Get quotes
 - Execute limit orders
 - Automatically adhere to rate limits

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
To [jejernig](http://stackoverflow.com/users/616499/jejernig) for [DevDefined.OAuth.Etrade](https://github.com/jejernig/DevDefined.OAuth---Etrade/network).  Excellent work!

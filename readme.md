#The Missing C# E*Trade Client

This is an (incomplete) fully managed client for the ETrade API.  So far it can:
 - Get accounts & balances
 - Get quotes
 - Execute limit orders
 - Automatically adhere to rate limits

See this page to get started:  [Getting Started](https://github.com/bmsapp/sappworks.stocks.public/wiki/Getting-Started)

##Example
```csharp
	// create the service client
	var client = new ETradeClient(_consumerToken);

	// get all accounts
	var accounts = client.GetAccounts();
```

```csharp

	// create some orders
	var orders =
		new List<Order>
		  {
			  new Order
			  {
				  IsSale = false, // buy order
				  Price = 32.00d,
				  Quantity = 100,
				  Symbol = "VA"
			  },
			  new Order
			  {
				  IsSale = true, // sell order
				  Price = 260.00d,
				  Quantity = 100,
				  Symbol = "TSLA"
			  }
		 };

	// execute them
	var response = client.ExecuteOrders(account.Id, orders);
```

##Special Thanks
To [jejernig](http://stackoverflow.com/users/616499/jejernig) for [DevDefined.OAuth.Etrade](https://github.com/jejernig/DevDefined.OAuth---Etrade/network).  Excellent work!

##MIT License

Copyright (c) 2012 Benjamin Sapp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE, ACCURACY, AND NONINFRINGEMENT. IN NO EVENT 
SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN	THE SOFTWARE.

###Terms of Use

Via this software ("Sappworks.Stocks") you are using the E*TRADE API, E*TRADE 
Developer Platform website and its contents ("Developer Platform").  You and, 
if applicable, the company you represent accept and agree to be bound by the 
terms, conditions, and disclosures established by E*TRADE including their 
"Terms of Use", "Application Programming Interface License Agreement", and 
their "Application Programming Interface User Agreement" or any other 
agreements they establish regarding the use of their software.

###Disclosures

All product names, logos, brands and other trademarks referred to within this
software, such as "E*Trade" or others, are the property of their 
respective trademark holders. These trademark holders are not affiliated with 
the developer(s) of this software, Sappworks, or Benjamin Sapp. E*TRADE does 
not sponsor or endorse our software.

This software contains programming examples.  All sample code is provided by 
for illustrative purposes only.  This software has not been thoroughly 
tested under all conditions.  This software is provided without guarantee 
or imply of reliability, serviceability, or function.

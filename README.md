# YenePaySDK - csharp

This library allows you to quickly and easily add YenePay as a payment method using C#

We encourage you to read through this README to get the most our of what this library has to offer. We want this library to be community driven and we really appreciate any support we can get from the community.

## Getting Started

These instructions will guide you on how to develop and test YenePay's payment method integration with your .NET application. We have setup a sandbox environment for you to test and play around the integration process. To learn more about this, please visit our community site: https://community.yenepay.com/

## Pre-requisite

To add YenePay to your application and start collecting payments, you will first need to register on YenePay as a merchant and get your seller code. You can do that from https://www.yenepay.com/merchant

## Installation

Step 1: Download the YenePaySDK.dll library under the folder YenePaySDK/YenePaySDK/bin/Release/, add it to your project files as a file and add a reference to this dll to your project.
To do this, open your project in Visual Studio, right click on References node under your project name, select Add References and locate the YenePaySDK.dll file.

The recommended way to install the latest version of the sdk is from nuget.org with the following Package Manager command

```
Install-Package YenePay.YenePaySDK
```

Step 2: Add a using reference to your class

```
using YenePaySDK;
```

Step 3: Generate a Checkout Url using the help methods provided by the SDK library as shown below

```
CheckoutOptions checkoutoptions = new CheckoutOptions("YOUR_SELLER_CODE", true);
```

This will create a new instance of type CheckoutOptions and sets the UseSandbox property to true. Set this to false when on production environment

Once you have that, set the other optional checkout options and provide the details of the order to be paid for.

```
checkoutoptions.Process = CheckoutType.Express; //alternatively you can set this to CheckoutType.Cart if you are including multiple items in a single order

// These properties are optional
checkoutoptions.SuccessReturn = "PAYMENT_SUCCESS_RETURN_URL";
checkoutoptions.CancelReturn = "PAYMENT_CANCEL_RETURN_URL";
checkoutoptions.IpnUrlReturn = "PAYMENT_COMPLETION_NOTIFICATION_URL";
checkoutoptions.FailureReturn = "PAYMENT_FAILURE_RETURN_URL";
checkoutoptions.ExpiresAfter = "NUMBER_OF_MINUTES_BEFORE_THE_ORDER_EXPIRES";
checkoutoptions.OrderId = "UNIQUE_ID_THAT_IDENTIFIES_THIS_ORDER_ON_YOUR_SYSTEM";

CheckoutItem checkoutitem = new CheckoutItem("NAME_OF_ITEM_PAID_FOR", UNIT_PRICE_OF_ITEM, QUANTITY);
string yenepayCheckoutUrl = CheckoutHelper.GetCheckoutUrl(checkoutoptions, checkoutitem);
```

Step 3: Redirect your customer to the checkout URL generated in step 2 above. Your customer will then be taken to our checkout page, login with his/her YenePay account and complete the payment there. Once a payment has been successfully completed, we will send you an Instant Payment Notification (IPN) to the URL you provided on the CheckoutOptions object in step 2. When you receive this notification, you should query our IPN verification url to make sure it is an authentic notification initiated by our servers.

A sample implementation of a method to receive and verify IPN is shown below

```
[HttpPost]
public void IPNDestination(IPNModel ipnModel)
{
	ipnModel.UseSandbox = true; // set to false on production environment
	if (ipnModel != null)
	{
		bool isIPNValid = CheckIPN(ipnModel).Result;

		if (isIPNValid)
		{
			//mark your order as "Paid" or "Completed" here
		}
	}
}

// verify ipn method
private async Task<bool> CheckIPN(IPNModel model)
{
	return await CheckoutHelper.IsIPNAuthentic(model);
}
```

## Deployment

When you are ready to take this to your production environment, just set the UseSandbox property of the CheckoutOptions object to false.






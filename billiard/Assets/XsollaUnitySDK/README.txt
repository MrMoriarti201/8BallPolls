Xsolla Inc. Unity SDK VERSION 1.2.3

GETTING STARTED

• To become an Xsolla partner, sign up in Xsolla control panel at this address: https://merchant.xsolla.com/signup/?utm_source=unity&utm_medium=instruction

• Familiarize yourself with the Integration Guides and select the modules that best fit your project. All the guides contain video tutorials on how to set up a Xsolla control panel for each specific module. You can find the Integration Guides here: https://developers.xsolla.com/#getting-started

• You have to chose way how to integrate GENERAL INTEGRATION or SIMPLE INTEGRATION. 

  GENERAL - require own backend and IPN realization. 
  SIMPLE - require only created project inside your merchant account but have less functionality.


GENERAL INTEGRATION

 Features:
 * Saved payment methods
 * User balance
            
• Set Up Instant Payment Notification (IPN): http://developers.xsolla.com/api.html#notifications

• Create an Xsolla Access Token to conduct payments with maximum security. You can find documentation on creating a token here: http://developers.xsolla.com/api.html#payment-ui

UI Xsolla

If you would like to accept payments through Xsolla’s payment UI, follow these steps:

• Add XsollaSDK script to any object or use prefab from “Resources -> Prefabs” folder;

• Call XsollaSDK(instance)to generate ready to use payment form. 

• CreatePaymentForm(token, actionOk(XsollaOkResult), actionError(XsollaError)) 

|————————————|————————————————————————————————————————————————————————-——————————————————————-|
|token	     |   Your purchase token                                                          |
|            |                                                                                |
|————————————|—————————————————————————————————————————————————————————————————————-—————————-| 
|actionOk    |   Call when payment process completed, delegate here your func                 |
|            |   example: OnResulOkReceivied(XsollaOkResult result){Debug.Log(“Ok”);}         |
|            |                                                                                |
|————————————|———————————————————————————————————————————————————————————————————————————————-|
|actionError |   Call when payment process canceled or some problems appeared,                |
|            |     delegate here your func                                                    | 
|            |   example: OnErrorReceivied(XsollaError data){Debug.Log(“Error”);}             |
|            |                                                                                |
|————————————|———————————————————————————————————————————————————————————————————————————————-|

• Also you can use XsollaSDK.InitPaystation(string token) function to use our payment solution in native browser.


Your payment UI


If you want have own payment UI you should write own class which extends XsollaPaystation Class.

As Example you can use XsollaPaystationController;


SIMPLE INTEGRATION

 Features:
 * Backend not required
 * Token generation not required
 * Less functionality
 * Less secure

To use SIMPLE INTEGRATION plase contact our account managers (am@xsolla.com)

UI Xsolla

• Add XsollaSDK script to any object or use prefab from “Resources -> Prefabs” folder;

• Call XsollaSDK(instance)to generate ready to use payment form. 

• CreatePaymentForm(xsollaJsonGenerator, actionOk(XsollaOkResult), actionError(XsollaError)) 
 
|————————————|————————————————————————————————————————————————————————-——————————————————————-|
| xsollaJG   |                                                                                |
|            |	XsollaJsonGenerator generator = new XsollaJsonGenerator ("userId", projectId);|
|			 |	generator.user.name = "John Smith";											  |
|			 |	generator.user.email = "support@xsolla.com";								  |
|			 |	generator.user.country = "US";												  |
|			 |	generator.settings.currency = "USD";										  |
|			 |	generator.settings.languge = "en";  										  |
|			 |	*generator.settings.mode = "sandbox";  										  |
|			 |	*generator.settings.secretKey = "BJJF93418FFJKLDFKSA;SDK";                     |
|            |               						                                          |
|————————————|———————————————————————————————————————————————————————————————————————————————-|
| actionOk   |   Call when payment process completed, delegate here your func                 |
|            |   example: OnResulOkReceivied(XsollaOkResult result){Debug.Log(“Ok”);}         |
|            |                                                                                |
|————————————|———————————————————————————————————————————————————————————————————————————————-|
| actionError|   Call when payment process canceled or some problems appeared,                |
|            |     delegate here your func                                                    | 
|            |   example: OnErrorReceivied(XsollaError data){Debug.Log(“Error”);}             |
|            |                                                                                |
|————————————|———————————————————————————————————————————————————————————————————————————————-|

	 * - to use mode sandbox for unsigned project, mode and secretKey(your project secretKey) params must be added. 

Your payment UI

If you want have own payment UI you should write own class which extends XsollaPaystation Class.

As Example you can use XsollaPaystationController;

SDK RESPONSE OBJECTS

public class XsollaResult {
    public string invoice{ get; set;}
    public Status status{ get; set;}
    // DONE status mean successful payment
    public Dictionary<string, object> purchases;
    // purchases can contain 
    // Key: «out» - virtual currency      | Value: int amount
    // Key: «id_package» - subscription   | Value: long subscriptionId
    // Key: «sku[itemId]» - virtual items | Value: int amount

}

public class XsollaError {
    public Source errorSource{ get; private set;}
    public int errorCode{ get; private set;}
    public string errorMessage{ get; private set;}
}


TRY IT!

You can look demo on https://livedemo.xsolla.com/sdk/unity/

We have two test scenes in "XsollaUnitySDK" -> "Resources" -> "_Scenes" folder:

• XsollaFarmFreshScene - emulates item shop

• XollaTokenTestScene - here you can test your token.

——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
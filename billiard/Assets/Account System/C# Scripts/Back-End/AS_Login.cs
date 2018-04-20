using UnityEngine;
using System;
using System.Collections;

public static class AS_Login
{
    const string loginPhp = "/checklogin.php?";
    const string getRegFormPhP = "/getregistrationform.php?";
    const string registerPhp = "/register.php?";
    const string passResetPhp = "/requestpasswordreset.php?";

    /// <summary>
    /// Downloads the registration form.
    /// </summary>
    /// <returns>The registration form.</returns>
    /// <param name="credentials">Credentials.</param>
    /// <param name="resultCallback">Result callback.</param>
    public static IEnumerator TryToDownloadRegistrationForm(AS_AccountInfo registrationForm, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of the registration script
        string url = hostUrl + getRegFormPhP;

		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();

		// Connect to the script
        WWW www = new WWW(url);

        // Wait for it to respond
        yield return www;
				

        // Check for WWW Errors
		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback(www.error);
            yield break;
        }
        // Check for PHP / MySQL Errors
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }
		if (www.text.ToLower().Contains("warning"))
        {
            Debug.LogWarning("AccountSystem: PHP/MySQL Warning:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }
		
		
		
        Debug.Log("AccountSystem: Received serialized registration form\n" + www.text);

        AS_AccountInfo temp = www.text.ToAccountInfo();
        registrationForm.fields = temp.fields;
        resultCallback("Registration Form downloaded Successfully!");

        yield break;

    }

    /// <summary>
    /// Retrieves the password.
    /// </summary>
    /// <returns>The password.</returns>
    /// <param name="credentials">Credentials.</param>
    /// <param name="resultCallback">Result callback.</param>
    public static IEnumerator TryToRecoverPassword(string email, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }
        // Location of the registration script
        string url = hostUrl + passResetPhp;
		
						
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("email", WWW.EscapeURL(email).Replace("@", "%40"));
        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback(www.error);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Debug.Log("AccountSystem: Emailed password reset link!\n" + www.text);
			resultCallback("Please check your email to reset the password!");
        }
        else
        {
            Debug.LogWarning("AccountSystem: Could not email password reset link - Check Message:\n" + www.text);
            resultCallback("Error: Could not email password reset link");
        }


        yield break;

    }


    /// <summary>
    /// Checks the registration fields.
    /// </summary>
    /// <returns><c>true</c>, if all fields cleared the check, <c>false</c> otherwise.</returns>
    /// <param name="regInfo">Reg info.</param>    
    /// <param name="registrationMessage"> Success / Error message message.</param>
    public static bool CheckFields(AS_AccountInfo regInfo,ref string errorMessage)
    {
		errorMessage = "";
		// Validate the data in the fields (make sure it matches their type
		if (regInfo.GetFieldValue("email") == null)
		{
			errorMessage = "Please input your email address.";
			return false;
		}
		if (regInfo.GetFieldValue("password") == null)
		{
			errorMessage = "Please input your password.";
			return false;
		}
		if (regInfo.GetFieldValue("NickName") == null)
		{
			errorMessage = "Please input your Nickname.";
			return false;
		}
		if (regInfo.GetFieldValue("Country") == null)
		{
			errorMessage = "Please choose your Country.";
			return false;
		}
		if ( !regInfo.fields.CheckMySQLFields (ref errorMessage) )
			return false;

		// All good..!
        return true;
    }


    /// <summary>
    /// Attempt to register to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToRegister(AS_AccountInfo newAccountInfo, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of the registration script
        string url = hostUrl + registerPhp;
		
						
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Hash the password
        string hashedPassword = newAccountInfo.fields.GetFieldValue("password").Hash();
        newAccountInfo.fields.SetFieldValue("password", hashedPassword);

        // If there should be an account activation, make sure we require it
        bool requireEmailActivation = (newAccountInfo.fields.GetIndex("isactive") >= 0);
        if (requireEmailActivation)
            newAccountInfo.fields.SetFieldValue("isactive", "0");

		// Serialize the custom info field
		newAccountInfo.SerializeCustomInfo ();

        // Serialize the account info
        string serializedAccountInfo = newAccountInfo.AccInfoToString(false);

        if (serializedAccountInfo == null)
        {
            Debug.LogError("AccountSystem: Could not serialize account info - check previous Debug.Log for errors");
            yield break;
        }
		
		form.AddField("newAccountInfo", serializedAccountInfo);

        form.AddField("requireEmailActivation", requireEmailActivation ? "true" : "false");

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Debug.Log("AccountSystem: New account registered successfully!\n" + www.text);
            resultCallback("New account registered successfully!");
        }
        else
        {
            Debug.LogError("AccountSystem: Could not register new account - Check Message:\n" + www.text);
            resultCallback("Error: Could not register new account");
        }


        yield break;


    }

    //Facebook
    public static IEnumerator TryToRegisterFB(AS_AccountInfo newAccountInfo, Action<string> resultCallback, string email, string nickname, string hostUrl = null )
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of the registration script
        string url = hostUrl + registerPhp;


        url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();

        // Create a new form
        WWWForm form = new WWWForm();

        // Hash the password
        string password = newAccountInfo.fields.GetFieldValue("password");
        string hashedPassword = newAccountInfo.fields.GetFieldValue("password").Hash();
        newAccountInfo.fields.SetFieldValue("password", hashedPassword);

        // If there should be an account activation, make sure we require it
        //bool requireEmailActivation = false;// (newAccountInfo.fields.GetIndex("isactive") >= 0);
       // if (requireEmailActivation)
       //     newAccountInfo.fields.SetFieldValue("isactive", "0");
        newAccountInfo.fields.SetFieldValue("isactive", "1");

        // Serialize the custom info field
        newAccountInfo.SerializeCustomInfo();

        // Serialize the account info
        string serializedAccountInfo = newAccountInfo.AccInfoToString(false);

        if (serializedAccountInfo == null)
        {
            Debug.LogError("AccountSystem: Could not serialize account info - check previous Debug.Log for errors");
            yield break;
        }

        form.AddField("newAccountInfo", serializedAccountInfo);

        form.AddField("requireEmailActivation", 1);

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

        
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Net.instance.OnLoginFB(email, password,nickname );
            //Debug.Log("AccountSystem: New account registered successfully!\n" + www.text);
            //resultCallback("New account registered successfully!");
        }
        else
        {
            Debug.LogError("AccountSystem: Could not register new account - Check Message:\n" + www.text);
            resultCallback("Error: Could not register new account");
        }


        yield break;


    }


    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    /// 
   

    public static IEnumerator TryToLoginFB(string username, string password, Action<string> resultCallback, string hostUrl = null, string Namefb = "")
	{
        Debug.Log("Login Facebook à! ------");
        Debug.Log("Password no hesh: " + password);
		if (hostUrl == null)
			hostUrl = AS_Credentials.phpScriptsLocation;

		if (hostUrl == "")
		{
			Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
			yield break;
		}
        string url = hostUrl + loginPhp;
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();		
		// Create a new form
		WWWForm form = new WWWForm();
		// Add The required fields
		form.AddField("username", username);
		// Hash the password
		string hashedPassword = password.Hash();
		form.AddField("password", hashedPassword);

		// Connect to the script
		WWW www = new WWW(url, form);

		// Wait for it to respond
		yield return www;
        Debug.Log("WWW - Debug  - " + www.text + "  :  " + www);
        Debug.Log("Debug hashedPassword -- " + hashedPassword);
        
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }
        if (www.text == "-1")
        {
            Debug.LogWarning("AccountSystem: Failed Login Attempt (Account Inactive)\n" + www.text);
            resultCallback("Error: Account is Inactive - Please check your emails.");
        }
        else
        if (www.text == "-2")
		{
            Debug.Log("Error Email/password");
            //Login.instance.RegInFB(username,"moriarti201billiard8", Namefb);
            Net.instance.OnRegisterBtnFacebook(username, "moriarti201billiard8", Namefb);
		} else
		{
			int id = -1;
			if (!int.TryParse(www.text, out id))
			{				
				resultCallback("Error: Could not connect. Please try again later!");
				Debug.LogError("AccountSystem: Failed Login Attempt (Unknown Error / Warning)\n" + www.text);
				yield break;
			}
			Debug.Log("AccountSystem: Successful Login for user with ID: " + www.text);
			resultCallback(www.text);
		}
	}

    public static IEnumerator TryToLogin(string username, string password, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Debug.LogError("AccountSystem: Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of our login script
        string url = hostUrl + loginPhp;
				
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();		
		
        // Create a new form
        WWWForm form = new WWWForm();

		// Add The required fields
        form.AddField("username", username);
        // Hash the password
        //string hashedPassword = "F0EB4A6677AA3DAF99F6F0DC514BB61D0E51AF74CFADFF4172F5720882CC1F4A23E9D577CE0C9B8305B65E6834302850A5423BAFEBDF448A77B6BA84695DDCE9";
        string hashedPassword = password.Hash();
        form.AddField("password", hashedPassword);
        // Connect to the script
		WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text == "-1")
        {
            Debug.LogWarning("AccountSystem: Failed Login Attempt (Account Inactive)\n" + www.text);
            resultCallback("Error: Account is Inactive - Please check your emails.");
        }
        else if (www.text == "-2")
        {
            Debug.LogWarning("AccountSystem: Failed Login Attempt (Invalid Email / Password)\n" + www.text);
            resultCallback("Error: Invalid Email / Password");
        }
        else
        {
			int id = -1;
			if (!int.TryParse(www.text, out id))
			{				
				resultCallback("Error: Could not connect. Please try again later!");
				Debug.LogError("AccountSystem: Failed Login Attempt (Unknown Error / Warning)\n" + www.text);
				yield break;
			}
			Debug.Log("AccountSystem: Successful Login for user with ID: " + www.text);
            resultCallback(www.text);
        }
    } 
}
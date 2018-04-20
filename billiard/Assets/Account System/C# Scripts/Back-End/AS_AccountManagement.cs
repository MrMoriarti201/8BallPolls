using UnityEngine;
using System;
using System.Collections;

public static class AS_AccountManagement
{
    public static void SetCustomInfo<T>(this AS_AccountInfo accountInfo, T customInfo, string customInfoFieldName = "custominfo")
    {
        // Serialize the custom info
        string serializedCustomInfo = customInfo.XmlSerializeToString();

        // Find the custom info field and update its value
        if (!accountInfo.fields.SetFieldValue(customInfoFieldName, serializedCustomInfo))
        {
            Debug.LogError("AccountSystem: Could not find custom info field " + customInfoFieldName);
            return;
        }

        // Debug.Log ("AccountSystem: Custom info field " + customInfoFieldName + " updated with new info (of type " + typeof(T).FullName + ")");

    }

    public static T GetCustomInfo<T>(this AS_AccountInfo accountInfo, string customInfoFieldName = "custominfo") where T : class, new()
    {
        // Find the custom info field
        int customInfoFieldIndex = accountInfo.fields.GetIndex(customInfoFieldName);

        if (customInfoFieldIndex < 0)
        {
            Debug.LogError("AccountSystem: Could not find custom info field " + customInfoFieldName);
            return new T();
        }

        // Get its value
        string customInfoXMLValue = accountInfo.fields[customInfoFieldIndex].stringValue;

        // If it's never set
        if (customInfoXMLValue == "")
        {
            Debug.LogWarning("AccountSystem: Custom info field " + customInfoFieldName + " hasn't been set.");
            return new T();
        }

        // Attempt to deserialize the text we got
        T customInfo = customInfoXMLValue.XmlDeserializeFromString<T>();
        // Debug.Log ("AccountSystem: Read info (of type " + typeof(T).FullName + ") from custom info field " + customInfoFieldName );
        return customInfo;

    }

    const string setAccountInfoPhp = "/setaccountinfo.php?";
    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToUploadAccountInfoToDb(int id, AS_AccountInfo accountInfo, System.Action<string> callback, string phpScriptsLocation = null)
    {

        if (phpScriptsLocation == null)
            phpScriptsLocation = AS_Credentials.phpScriptsLocation;

        if (phpScriptsLocation == "")
        {
            Debug.LogError("AccountSystem: PHP Scripts Location not set..! Please load the Setup scene located in ../AccountSystem/Setup/");
            yield break;
        }

        Debug.Log("AccountSystem: Uploading Account Info for user with id " + id);


        // Location of our download info script
        string url = phpScriptsLocation + setAccountInfoPhp;
						
		
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("id", WWW.EscapeURL(id.ToString()));

		// Serialize the customInfo field
		if(!accountInfo.SerializeCustomInfo())
		{
			Debug.LogError("AccountSystem: Could not serialize custom info - check previous Debug.Log for errors");
			yield break;

		}
        string serializedAccountInfo = accountInfo.AccInfoToString(true);

        if (serializedAccountInfo == null)
        {
            Debug.LogError("AccountSystem: Could not serialize account info - check previous Debug.Log for errors");
            yield break;
        }

        form.AddField("info", serializedAccountInfo);

		
        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        Debug.Log("AccountSystem: Awaiting response from: " + url);
        yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            callback("error: " + www.error);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            callback("error: " + www.text);
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Debug.Log("AccountSystem: Account Info uploaded successfully for user with id " + id);
            callback("Account Info uploaded successfully for user with id " + id);
            yield break;
        }
    }


    const string getAccountInfoPhp = "/getaccountinfo.php?";
    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToDownloadAccountInfoFromDb(int id, AS_AccountInfo accountInfo, System.Action<string> callback, string phpScriptsLocation = null)
    {

        if (phpScriptsLocation == null)
            phpScriptsLocation = AS_Credentials.phpScriptsLocation;

        if (phpScriptsLocation == "")
        {
            Debug.LogError("PHP Scripts Location not set..! Please load the Setup scene located in ../AccountSystem/Setup/");
            yield break;
        }

        Debug.Log("AccountSystem: Downloading Account Info for user with id " + id);


        // Location of our download info script
        string url = phpScriptsLocation + getAccountInfoPhp;
				
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("id", WWW.EscapeURL(id.ToString()));

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        Debug.Log("AccountSystem: Awaiting response from: " + url);
        yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("AccountSystem: WWW Error:\n" + www.error);
            callback("error: " + www.text);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Debug.LogError("AccountSystem: PHP/MySQL Error:\n" + www.text);
            callback("error: " + www.text);
            yield break;
        }

        //Debug.Log(www.text);

        // Attempt to deserialize the text we got
        AS_AccountInfo temp = www.text.ToAccountInfo();
        accountInfo.fields = temp.fields;
		if (!accountInfo.DeSerializeCustomInfo ())
			Debug.LogWarning("AccountSystem: Could not deserialize Custom Info");

        callback("Account Info downloaded successfully for user with id " + id);

    }

}
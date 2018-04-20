using System;
using System.Collections;
using UnityEngine;

public class Net : NetBase {

	//public const string url = "http://flamingotouch.com/Billiard/";
	public const string url = "http://ets-demo.com/b/billiard/";
	//public const string url = "http://www.abayasdesign.com/Billiard/";
	public static Net instance;

	// Registration Variables
	AS_AccountInfo accountInfo = new AS_AccountInfo();

	string guiMessage = "";
	
	// Login Variables
	[HideInInspector]
	public string usernameLogin = "";
	[HideInInspector]
	public string passwordLogin = ""; 
	 
	protected override void Awake() {
		instance=this;
		base.Awake(); 
	}

	protected override void Update () {	
		base.Update();
	}

	#region Callbacks
	// Called by the AttemptLogin coroutine when it's finished executing
	public void LoginAttempted(string callbackMessage)
	{
		Debug.Log(callbackMessage);
		// If our log in failed,   
		if (callbackMessage.IsAnError())
		{
			Debug.LogError(callbackMessage);
			return;
		
		}        
		// Otherwise,
		int id = Convert.ToInt32(callbackMessage);
		GlobalInfo.myProfile.user_id=id;        
		Login.instance.StartGame();
	}
	
	// Used by the AttemptDownloadRegistrationForm when it's finished executing
	void RegistrationFormDownloaded(string callbackMessage)
	{
		Debug.Log("Register ready:"+callbackMessage);
		if (callbackMessage.IsAnError())
		{
			Debug.LogError(callbackMessage);
			return;
		}
	}

	// Called by the AttemptRegistration coroutine when it's finished executing
	public void RegistrationAttempted(string callbackMessage)
	{
		Debug.Log(callbackMessage);
		// If our registration failed,
		if (callbackMessage.IsAnError())
		{
			Debug.LogError(callbackMessage);
			return;
		}
        GameManager.instance.LoadScene("Login");
		GameManager.instance.PopupMessage("Please check your email and activate your account");
	}
    public void RegistrationAttemptedFB(string callbackMessage)
    {
        Debug.Log(callbackMessage);
        // If our registration failed,
        if (callbackMessage.IsAnError())
        {
            
            return;
        }
        GameManager.instance.LoadScene("Login");
        GameManager.instance.PopupMessage("Please check your email and activate your account");
    }

    // Called by the AttemptPasswordRecovery coroutine when it's finished executing
    public void PasswordRecoveryAttempted(string callbackMessage)
	{
		Debug.Log(callbackMessage);
		// If our registration failed,
		if (callbackMessage.IsAnError())
		{
			Debug.LogError(callbackMessage);
			return;
		}
		GameManager.instance.LoadScene("Login");
		GameManager.instance.PopupMessage(callbackMessage);
	}
	#endregion
	
	// Called by OnGUI and provides a basic login GUI layout
	public void OnLogin(string username, string pass){ 
		usernameLogin = username;
		passwordLogin = pass;
        usernameLogin.TryToLogin(passwordLogin, LoginAttempted);
        // Equivalent to: 

        //AS_Login.ChekedFB(accountInfo);
		//StartCoroutine ( AS_Login.TryToLoginFB( usernameLogin, passwordLogin, LoginAttempted ) ) ;
		Debug.Log("Attempting to Log In.." + usernameLogin + ":" + passwordLogin);
		GameManager.instance.ShowLoading(true);
	}
	public void OnLoginFB(string username, string pass, string name = ""){
		usernameLogin = username;
		passwordLogin = pass;
        
		//usernameLogin.TryToLogin(passwordLogin, LoginAttempted);
		StartCoroutine(AS_Login.TryToLoginFB(username:username, password: pass,resultCallback: LoginAttempted, Namefb:name));
		// Equivalent to: 
		// StartCoroutine ( AS_Login.TryToLogin( usernameLogin, passwordLogin, LoginAttempted ) ) ;
		Debug.Log("Attempting to Log In.." + usernameLogin + ":" + passwordLogin);
		GameManager.instance.ShowLoading(true);	
	}

    public void NetTest()
    {
        usernameLogin = "aa";        
        usernameLogin.TryToLogin("aa", LoginTest);        
        Debug.Log("Attempting to Log In.." + usernameLogin + ":" + passwordLogin);        
    }

    public void LoginTest(string callbackMessage)
    {
        Debug.Log(callbackMessage);
        // If our log in failed,   
        if (callbackMessage.IsAnError())
        {
            Debug.LogError(callbackMessage);
            return;
        }
    }	

	public void Register() {
		// When the form is downloaded, RegistrationFormDownloaded is called
		accountInfo.TryToDownloadRegistrationForm(RegistrationFormDownloaded);
		// Equivalent to: 
		// StartCoroutine ( AS_Login.TryToDownloadRegistrationForm (accountInfo, RegistrationFormDownloaded) );
	}

	public void OnRegisterBtn(string email,string pass,string nickName, string country){
		accountInfo.fields.SetFieldValue("username",email);
		accountInfo.fields.SetFieldValue("email",email);        
		accountInfo.fields.SetFieldValue("password",pass);
		accountInfo.fields.SetFieldValue("NickName",nickName);
		accountInfo.fields.SetFieldValue("Country",country);

        Debug.Log("выполнение registerBTN");
		if (!AS_Login.CheckFields(accountInfo,ref guiMessage))
		{
			GameManager.instance.PopupMessage(guiMessage);
		return;
		}

		// Online check with the given database
		guiMessage = "Attempting to Register..";
		accountInfo.TryToRegister(RegistrationAttempted);
		// Equivalent to: 
		// StartCoroutine ( AS_Login.TryToRegister( accountInfo, RegistrationAttempted ) ) ;
		GameManager.instance.ShowLoading(true);
        Debug.Log("выполнился register");
    }
	public void OnRegisterBtnFacebook(string email,string pass,string nickName){
        Register();
        accountInfo.fields.SetFieldValue("username", email);
        accountInfo.fields.SetFieldValue("email", email);
        accountInfo.fields.SetFieldValue("password", pass);
        accountInfo.fields.SetFieldValue("NickName", nickName);
        accountInfo.fields.SetFieldValue("Country", "1");


        Debug.Log ("OnRegisterBtn - -- " + email);
		if (!AS_Login.CheckFields(accountInfo,ref guiMessage))
		{
			GameManager.instance.PopupMessage(guiMessage);
			return;
		}
		// Online check with the given database
		guiMessage = "Attempting to Register..";
        //accountInfo.TryToRegister(RegistrationAttempted);
        StartCoroutine(AS_Login.TryToRegisterFB(accountInfo, RegistrationAttempted, email, nickName));
        // Equivalent to: 
		// StartCoroutine ( AS_Login.TryToRegister( accountInfo, RegistrationAttempted ) ) ;
		GameManager.instance.ShowLoading(true);
	}

    
	string emailPasswordRecovery = "";

	public void PasswordRecovery(string email){
		emailPasswordRecovery = email;
		emailPasswordRecovery.TryToRecoverPassword(PasswordRecoveryAttempted);
		// Equivalent to: 
		// StartCoroutine(AS_Login.TryToRecoverPassword ( emailPasswordRecovery, PasswordRecoveryAttempted ) );
		GameManager.instance.ShowLoading(true);
	}

    public void StartOnlieStatus()
    {
        StartCoroutine(OnlineStatus());
    }
    public IEnumerator OnlineStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            SendMsg(new UserManager.OnlineStatus());            
        }
    }

    public int tt = 0;
}
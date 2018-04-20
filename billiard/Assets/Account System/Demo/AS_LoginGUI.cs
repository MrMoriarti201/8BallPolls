using UnityEngine;
using System;
using System.Collections;

public class AS_LoginGUI : MonoBehaviour
{

    /*
     * 		--- TODO: CUSTOMIZE ---
     * 
     * Called upon Successful Login - Add any custom logic here
     * You could load a level, initialize some characters or
     * download additional info. The latter can be done by an
     * AS_AccountManagementGUI instance.
    */
    public void SuccessfulLogin(int id)
    {
        Debug.Log("AccountSystem: Successfully Logged In User with id: " + id + " - Add any custom Logic Here!");
        loginState = AS_LoginState.LoginSuccessful;

        if (accountManagementGUI)
        {
            accountManagementGUI.enabled = true;
            accountManagementGUI.Init(id);
        }
    }

    // This can be activated once we have successfully logged in
    AS_AccountManagementGUI accountManagementGUI;

    // Registration Variables
    AS_AccountInfo accountInfo = new AS_AccountInfo();
    string passwordPrompt = "";
    string passwordConfirm = "";
    string emailConfirm = "";
    string guiMessage = "";

    // Login Variables
    string usernameLogin = "";
    string passwordLogin = "";


    void Start()
    {

        // Connect to the account management GUI
        accountManagementGUI = this.GetComponent<AS_AccountManagementGUI>();
        if (accountManagementGUI)
            accountManagementGUI.enabled = false;
    }


    // If the state changes update messages / load level
    AS_LoginState _loginState = AS_LoginState.LoginPrompt;
    AS_LoginState loginState
    {
        get { return _loginState; }
        set
        {
            _loginState = value;
            guiMessage = "";
        }
    }

    void OnGUI()
    {

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10));

        // Login
        if (loginState == AS_LoginState.LoginPrompt)
            LoginGUI(AS_Preferences.enablePasswordRecovery);

        // Successful Log In
        else if (loginState == AS_LoginState.LoginSuccessful)
            LoggedInGUI();

        // Password Recovery
        else if (loginState == AS_LoginState.RecoverPassword)
            PasswordRecoveryGUI();

        // Registration
        else if (loginState == AS_LoginState.Registering)
            RegistrationGUI();

        GUILayout.EndArea();

    }

    #region Callbacks
    // Called by the AttemptLogin coroutine when it's finished executing
    public void LoginAttempted(string callbackMessage)
    {

        guiMessage = callbackMessage;

        // If our log in failed,
        if (callbackMessage.IsAnError())
        {
            Debug.LogError(callbackMessage);
            return;
        }

        // Otherwise,
        int accountId = Convert.ToInt32(callbackMessage);
        SuccessfulLogin(accountId);

    }

    // Used by the AttemptDownloadRegistrationForm when it's finished executing
    void RegistrationFormDownloaded(string callbackMessage)
    {

        guiMessage = callbackMessage;

        if (callbackMessage.IsAnError())
        {
            Debug.LogError(callbackMessage);
            return;
        }

        loginState = AS_LoginState.Registering;

		// What you want to appear in the registration GUI
		guiMessage = "Please fill in the required fields.";

    }

    // Called by the AttemptRegistration coroutine when it's finished executing
    public void RegistrationAttempted(string callbackMessage)
    {
		
		guiMessage = callbackMessage;

        // If our registration failed,
        if (callbackMessage.IsAnError())
        {
            Debug.LogError(callbackMessage);
            return;
        }

        // Otherwise,
        loginState = AS_LoginState.LoginPrompt;
		
		guiMessage = callbackMessage;

    }

    // Called by the AttemptPasswordRecovery coroutine when it's finished executing
    public void PasswordRecoveryAttempted(string callbackMessage)
    {
		
		guiMessage = callbackMessage;

        // If our registration failed,
        if (callbackMessage.IsAnError())
        {
            Debug.LogError(callbackMessage);
            return;
        }

        // Otherwise,
        loginState = AS_LoginState.LoginPrompt;
		
		guiMessage = callbackMessage;

    }
    #endregion

    // Called by OnGUI and provides a basic login GUI layout
    void LoginGUI(bool enablePasswordRecovery)
    {

        // Tittle
        GUILayout.Label("~~~==== Login ====~~~", GUILayout.Width(300));
        GUILayout.Label(" ", GUILayout.Width(100));

        // Username Propmpt
        GUILayout.BeginHorizontal();
        GUILayout.Label("Username: ", GUILayout.Width(200));
        usernameLogin = GUILayout.TextField(usernameLogin, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        // Password Prompt
        GUILayout.BeginHorizontal();
        GUILayout.Label("Password: ", GUILayout.Width(200));
        passwordLogin = GUILayout.TextField(passwordLogin, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        // Message handling (invalid username & password / inactive account / .. )
        GUILayout.Label(guiMessage, GUILayout.Width(300));

        // Login Button - Attempts to Log in
        if (GUILayout.Button("Login", new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) }))
        {

            // Calls the appropriate php script (its location is specified from the crentials)
            // with the following arguments {username, password} and when it gets a response, 
            // it calls the function "setLoginState" with the appropriate arguments
			usernameLogin.TryToLogin(passwordLogin, LoginAttempted);

            // Equivalent to: 
			// StartCoroutine ( AS_Login.TryToLogin( usernameLogin, passwordLogin, LoginAttempted ) ) ;

			guiMessage = "Attempting to Log In..";

        }


        // Register Button - Attempts to download the registration form (which fields are required in order to log in
        if (GUILayout.Button("Register", new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) }))
        {
            passwordPrompt = "";
            passwordConfirm = "";
            emailConfirm = "";
			// When the form is downloaded, RegistrationFormDownloaded is called
            accountInfo.TryToDownloadRegistrationForm(RegistrationFormDownloaded);
            // Equivalent to: 
			// StartCoroutine ( AS_Login.TryToDownloadRegistrationForm (accountInfo, RegistrationFormDownloaded) );

			guiMessage = "Loading..";
        }

        // Password Recovery screen
        if (enablePasswordRecovery)
            if (GUILayout.Button("Retrieve Password", new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) }))
                loginState = AS_LoginState.RecoverPassword;

        // Tutorial
#if UNITY_EDITOR
		GUILayout.Label ("", GUILayout.Height(10));
		GUILayout.Label ("\bGetting Started:" +
			"\n1) From the Menu Bar select Window->Online Account System" +
			"\n2) Follow the Setup Guide" +
			"\n3) Try to Register and then Login to Manage your Account" +
			"\n\nThis message was printed from AS_LoginGUI.cs", GUILayout.Width(500));
#endif
    }

    // Called by OnGUI and provides a basic registration GUI layout
    void RegistrationGUI()
    {

        // Title
        GUILayout.Label("~~~==== Registration ====~~~", GUILayout.Width(300));
        GUILayout.Label(" ", GUILayout.Width(100));

        // Registration Info has the fields the user should fill in
        foreach (AS_MySQLField field in accountInfo.fields)
        {

            // Id is an auto-increment unique identifier
            // and custom info is not specified during registration
            if (field.name.ToLower() == "id" | field.name.ToLower() == "custominfo" | field.name.ToLower() == "isactive")
                continue;

            // For any given field
            GUILayout.BeginHorizontal();

            // Print the name
            // Check if it's required
            string msgRequired = field.isRequired ? "\t\b *" : "";
            GUILayout.Label(field.name.UppercaseFirst() + msgRequired, GUILayout.Width(200));

            // Prompt the user to input the value
            // And store the value	
            string tempVal = "";
            if (field.name.ToLower().Contains("password"))
            {
                passwordPrompt = GUILayout.TextField(passwordPrompt, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });
                accountInfo.fields.SetFieldValue(field.name, passwordPrompt);
            }

            else
            {
                tempVal = GUILayout.TextField(accountInfo.fields.GetFieldValue(field.name), new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });
                accountInfo.fields.SetFieldValue(field.name, tempVal);
            }

            GUILayout.EndHorizontal();


            // Additionally, if it's the password or email
            // Ask for confirmation
            if (field.name.ToLower().Contains("password"))
            {

                GUILayout.BeginHorizontal();

                GUILayout.Label("Repeat your Password:", GUILayout.Width(200));

                passwordConfirm = GUILayout.TextField(passwordConfirm,
                    new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });

                GUILayout.EndHorizontal();
            }
            if (field.name.ToLower().Contains("email"))
            {

                GUILayout.BeginHorizontal();

                GUILayout.Label("Repeat your Email:", GUILayout.Width(200));

                emailConfirm = GUILayout.TextField(emailConfirm,
                    new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });

                GUILayout.EndHorizontal();
            }

        }

        GUILayout.Label(" ", GUILayout.Height(10));
        GUILayout.Label("*: Required Field", GUILayout.Width(300));
        GUILayout.Label(" ", GUILayout.Height(10));

        // For errors ( username taken etc.. )
        GUILayout.Label(guiMessage);
        GUILayout.Label(" ", GUILayout.Height(10));

        // Submit registration button
        if (GUILayout.Button("Register",
            new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(50) }))
        {

            // Offline field check
			if (!AS_Login.CheckFields(accountInfo,ref guiMessage))// passwordConfirm, emailConfirm, ref guiMessage))
            {
                return;
            }

            // Online check with the given database
			guiMessage = "Attempting to Register..";
            accountInfo.TryToRegister(RegistrationAttempted);
            // Equivalent to: 
            // StartCoroutine ( AS_Login.TryToRegister( accountInfo, RegistrationAttempted ) ) ;
        }

        // Return to Login
        if (GUILayout.Button("Back",
            new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(40) }))
        {
            usernameLogin = "";
            passwordLogin = "";
            loginState = AS_LoginState.LoginPrompt;
        }

    }

    // Called by OnGUI and provides a basic registration GUI layout
    string emailPasswordRecovery = "";
    void PasswordRecoveryGUI()
    {

        // Title
        GUILayout.Label("~~~==== Password Recovery ====~~~", GUILayout.Width(300));
        GUILayout.Label(" ", GUILayout.Width(100));

        // Email address prompt
        GUILayout.Label("Enter your account's email address:", GUILayout.Width(200));
        emailPasswordRecovery = GUILayout.TextField(emailPasswordRecovery, GUILayout.Width(200));

        // Send email button
        if (GUILayout.Button("Email password reset link",
                new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(40) }))
        {
            emailPasswordRecovery.TryToRecoverPassword(PasswordRecoveryAttempted);
            // Equivalent to: 
            // StartCoroutine(AS_Login.TryToRecoverPassword ( emailPasswordRecovery, PasswordRecoveryAttempted ) );
			
			guiMessage = "Processing your request..";
        }
        // Return to Login
        if (GUILayout.Button("Back",
              new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) }))
            loginState = AS_LoginState.LoginPrompt;
		
		// For errors ( username taken etc.. )
		GUILayout.Label(guiMessage);
		GUILayout.Label(" ", GUILayout.Height(10));

    }

    // Called by OnGUI and provides a Logout option
    void LoggedInGUI()
    {

        if (GUI.Button(new Rect(40, Screen.height - 60, 150, 30), "Logout"))
        {

            // If we have an account management GUI, enable it
            if (accountManagementGUI)
                accountManagementGUI.enabled = false;

            usernameLogin = "";
            passwordLogin = "";

            loginState = AS_LoginState.LoginPrompt;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.HTTPNetworking;
using DevelopersHub.HTTPNetworking.Tools;
using UnityEngine.UI;
using System.Linq;
using System;

public class AuthenticationDemo1 : MonoBehaviour
{

    [Header("Main")]
    [SerializeField] public Image background = null;
    [SerializeField] private AuthenticationDemo2 accountScript = null;

    [Header("Landing")]
    [SerializeField] private GameObject landing = null;
    [SerializeField] private GameObject landingPortrait = null;
    [SerializeField] private GameObject landingLandscape = null;
    [SerializeField] private Button landingPortraitSignIn = null;
    [SerializeField] private Button landingPortraitSignUp = null;
    [SerializeField] private Button landingLandscapeSignIn = null;
    [SerializeField] private Button landingLandscapeSignUp = null;

    [Header("Sign In")]
    [SerializeField] private GameObject signin = null;
    [SerializeField] private GameObject signinPortrait = null;
    [SerializeField] private GameObject signinLandscape = null;
    [SerializeField] private InputField signinPortraitUsername = null;
    [SerializeField] private InputField signinPortraitPassword = null;
    [SerializeField] private InputField signinLandscapeUsername = null;
    [SerializeField] private InputField signinLandscapePassword = null;
    [SerializeField] private Button signinPortraitSignIn = null;
    [SerializeField] private Button signinLandscapeSignIn = null;
    [SerializeField] private Button signinPortraitForgoten = null;
    [SerializeField] private Button signinLandscapeForgoten = null;
    [SerializeField] private Button signinPortraitBack = null;
    [SerializeField] private Button signinLandscapeBack = null;

    [Header("Sign Up")]
    [SerializeField] private GameObject signup = null;
    [SerializeField] private GameObject signupPortrait = null;
    [SerializeField] private GameObject signupLandscape = null;
    [SerializeField] private InputField signupPortraitUsername = null;
    [SerializeField] private InputField signupPortraitPassword = null;
    [SerializeField] private InputField signupPortraitEmail = null;
    [SerializeField] private InputField signupLandscapeUsername = null;
    [SerializeField] private InputField signupLandscapePassword = null;
    [SerializeField] private InputField signupLandscapeEmail = null;
    [SerializeField] private Button signupPortraitsignup = null;
    [SerializeField] private Button signupLandscapesignup = null;
    [SerializeField] private Button signupPortraitBack = null;
    [SerializeField] private Button signupLandscapeBack = null;

    [Header("Recovery Code")]
    [SerializeField] private GameObject recovery = null;
    [SerializeField] private GameObject recoverPortrait = null;
    [SerializeField] private GameObject recoverLandscape = null;
    [SerializeField] private InputField emailRecoverPortrait = null;
    [SerializeField] private InputField emailRecoverLandscape = null;
    [SerializeField] private InputField phoneRecoverPortrait = null;
    [SerializeField] private InputField phoneRecoverLandscape = null;
    [SerializeField] private Dropdown countryRecoverPortrait = null;
    [SerializeField] private Dropdown countryRecoverLandscape = null;
    [SerializeField] private Button emailRecoverPortraitSend = null;
    [SerializeField] private Button emailRecoverLandscapeSend = null;
    [SerializeField] private Button emailRecoverPortraitBack = null;
    [SerializeField] private Button emailRecoverLandscapeBack = null;
    [SerializeField] private Toggle recoverPortraitEmail = null;
    [SerializeField] private Toggle recoverLandscapeEmail = null;
    [SerializeField] private Toggle recoverPortraitPhone = null;
    [SerializeField] private Toggle recoverLandscapePhone = null;
    [SerializeField] private Text recoverPortraitCountryCode = null;
    [SerializeField] private Text recoverLandscapeCountryCode = null;

    [Header("Change Password")]
    [SerializeField] private GameObject changePass = null;
    [SerializeField] private GameObject changePassPortrait = null;
    [SerializeField] private GameObject changePassLandscape = null;
    [SerializeField] private InputField changePassPortraitCode = null;
    [SerializeField] private InputField changePassLandscapeCode = null;
    [SerializeField] private InputField changePassPortraitPass = null;
    [SerializeField] private InputField changePassLandscapePass = null;
    [SerializeField] private Button changePassPortraitChange = null;
    [SerializeField] private Button changePassLandscapeChange = null;
    [SerializeField] private Button changePassPortraitBack = null;
    [SerializeField] private Button changePassLandscapeBack = null;
    [SerializeField] private Text changePassPortraitAddress = null;
    [SerializeField] private Text changePassLandscapeAddress = null;
    [SerializeField] private Text changePassPortraitTime = null;
    [SerializeField] private Text changePassLandscapeTime = null;

    private enum Page
    {
        none, landing, signin, signup, recovery, password
    }

    private Page page = Page.landing;
    private bool lastPortrait = false;
    private bool signingIn = false;
    private bool signingUp = false;

    private string username = "";
    private string password = "";
    private string email = "";
    private string phoneNumer = "";
    private string phoneCountry = "";
    private string code = "";

    private float codeLife = 0;

    private void Awake()
    {
        accountScript.background.SetActive(false);
        background.gameObject.SetActive(true);
        Networking.OnConnectedToServerSuccessful += HTTPNetworking_OnConnectedToServerSuccessful;
        Networking.OnConnectToServerFailed += HTTPNetworking_OnConnectToServerFailed;
        Networking.OnDisconnectedFromServer += HTTPNetworking_OnDisconnectedFromServer;
    }

    private void Start()
    {
        Authentication.Initialize();

        MessageBox.Instance.onButton1Clicked.AddListener(MessageBoxButton1_Clicked);
        MessageBox.Instance.onButton2Clicked.AddListener(MessageBoxButton2_Clicked);
        UiEvents.Instance.OnRectTransformSizeChange.AddListener(OnScreenSizeChanged);
        OnScreenSizeChanged();

        SetCountryCode(countryRecoverPortrait);
        SetCountryCode(countryRecoverLandscape);
        OnRecoverCountryChanged(0);

        landingPortraitSignIn.onClick.AddListener(LandingSignIn);
        landingLandscapeSignIn.onClick.AddListener(LandingSignIn);
        landingPortraitSignUp.onClick.AddListener(LandingSignUp);
        landingLandscapeSignUp.onClick.AddListener(LandingSignUp);

        signinPortraitSignIn.onClick.AddListener(SignInCheck);
        signinLandscapeSignIn.onClick.AddListener(SignInCheck);

        signupPortraitsignup.onClick.AddListener(SignUpCheck);
        signupLandscapesignup.onClick.AddListener(SignUpCheck);

        signinPortraitForgoten.onClick.AddListener(SignInForgotten);
        signinLandscapeForgoten.onClick.AddListener(SignInForgotten);

        emailRecoverPortraitSend.onClick.AddListener(SendCodeCheck);
        emailRecoverLandscapeSend.onClick.AddListener(SendCodeCheck);

        changePassPortraitChange.onClick.AddListener(ChangePasswordCheck);
        changePassLandscapeChange.onClick.AddListener(ChangePasswordCheck);

        signinPortraitBack.onClick.AddListener(Back);
        signinLandscapeBack.onClick.AddListener(Back);
        signupPortraitBack.onClick.AddListener(Back);
        signupLandscapeBack.onClick.AddListener(Back);
        emailRecoverPortraitBack.onClick.AddListener(Back);
        emailRecoverLandscapeBack.onClick.AddListener(Back);
        changePassPortraitBack.onClick.AddListener(Back);
        changePassLandscapeBack.onClick.AddListener(Back);

        recoverPortraitEmail.onValueChanged.AddListener(OnRecoverWithEmail);
        recoverLandscapeEmail.onValueChanged.AddListener(OnRecoverWithEmail);
        recoverPortraitPhone.onValueChanged.AddListener(OnRecoverWithEmail);
        recoverLandscapePhone.onValueChanged.AddListener(OnRecoverWithEmail);

        countryRecoverPortrait.onValueChanged.AddListener(OnRecoverCountryChanged);
        countryRecoverLandscape.onValueChanged.AddListener(OnRecoverCountryChanged);

        EmailPhoneSwitch();
        OpenPage(Page.landing);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
        if(codeLife > 0)
        {
            codeLife -= Time.fixedUnscaledDeltaTime;
            if(codeLife < 0)
            {
                codeLife = 0;
            }
            TimeSpan ts = TimeSpan.FromSeconds(codeLife);
            changePassPortraitTime.text = "Code expires in " + string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
            changePassLandscapeTime.text = changePassPortraitTime.text;
        }
    }

    private void GoToAccountScene()
    {
        Authentication.OnGetUserDataFailed += Authentication_OnGetUserDataFailed;
        Authentication.OnGetUserDataSuccessful += Authentication_OnGetUserDataSuccessful;
        Authentication.GetUserData(Authentication.UserID);
    }

    private void Authentication_OnGetUserDataSuccessful(LitJson.JsonData data)
    {
        Authentication.OnGetUserDataFailed -= Authentication_OnGetUserDataFailed;
        Authentication.OnGetUserDataSuccessful -= Authentication_OnGetUserDataSuccessful;
        background.gameObject.SetActive(false);
        accountScript.Initialize(data["username"].ToString(), int.Parse(data["is_verified"].ToString()) > 0, int.Parse(data["score"].ToString()));
        MessageBox.ShowMessage("SUCCESS!", "You logged in to your account successfully.", MessageBox.Type.success, "OK");
    }

    private void Authentication_OnGetUserDataFailed(string error)
    {
        Authentication.OnGetUserDataFailed -= Authentication_OnGetUserDataFailed;
        Authentication.OnGetUserDataSuccessful -= Authentication_OnGetUserDataSuccessful;
        MessageBox.ShowMessage("ERROR!", "Failed to access acount data.", MessageBox.Type.error, "OK");
    }

    private void LandingSignIn()
    {
        OpenPage(Page.signin);
    }

    private void LandingSignUp()
    {
        OpenPage(Page.signup);
    }

    private void ChangePasswordCheck()
    {
        if (Screen.width < Screen.height)
        {
            code = changePassPortraitCode.text;
            password = changePassPortraitPass.text;
        }
        else
        {
            code = changePassLandscapeCode.text;
            password = changePassLandscapePass.text;
        }
        if (string.IsNullOrEmpty(code))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Code can not be empty.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }

        DevelopersHub.HTTPNetworking.Tools.Loading.Show();
        Authentication.OnChangePasswordFailed += Authentication_OnChangePasswordFailed;
        Authentication.OnChangePasswordSuccessful += Authentication_OnChangePasswordSuccessful;
        Authentication.ChangePassword(code, password, email, phoneNumer);
    }

    private void Authentication_OnChangePasswordSuccessful(string username)
    {
        Authentication.OnChangePasswordFailed -= Authentication_OnChangePasswordFailed;
        Authentication.OnChangePasswordSuccessful -= Authentication_OnChangePasswordSuccessful;
        DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
               "DONE!",
               "Password changed successfully. Your usernames is " + username + " and you can log in to your account with the password that you just choosed.",
               DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.success,
               "OK");
        OpenPage(Page.signin);
        signinPortraitUsername.text = username;
        signinLandscapeUsername.text = username;
        DevelopersHub.HTTPNetworking.Tools.Loading.Hide();
    }

    private void Authentication_OnChangePasswordFailed(string error)
    {
        Authentication.OnChangePasswordFailed -= Authentication_OnChangePasswordFailed;
        Authentication.OnChangePasswordSuccessful -= Authentication_OnChangePasswordSuccessful;
        string er = "The code is not valid.";
        if (error == "ERROR_PASSWORD_LENGHT")
        {
            er = "Password must be minimum " + Authentication.Instance.PasswordMinLenght.ToString() + " and maximum " + Authentication.Instance.PasswordMaxLenght.ToString() + " characters.";
        }
        DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
           "ERROR!",
           er,
           DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
           "OK");
        DevelopersHub.HTTPNetworking.Tools.Loading.Hide();
    }

    private void SendCodeCheck()
    {
        bool em = true;
        if (Screen.width < Screen.height)
        {
            if (recoverPortraitEmail.isOn)
            {
                email = emailRecoverPortrait.text.Trim();
                phoneNumer = null;
                phoneCountry = null;
            }
            else
            {
                phoneNumer = recoverPortraitCountryCode.text + phoneRecoverPortrait.text.Trim();
                phoneCountry = countryCodes.ElementAt(countryRecoverPortrait.value).Key.ToLower();
                email = null;
                em = false;
            }
        }
        else
        {
            if (recoverLandscapeEmail.isOn)
            {
                email = emailRecoverLandscape.text.Trim();
                phoneNumer = null;
                phoneCountry = null;
            }
            else
            {
                phoneNumer = recoverLandscapeCountryCode + phoneRecoverLandscape.text.Trim();
                phoneCountry = countryCodes.ElementAt(countryRecoverLandscape.value).Key.ToLower();
                email = null;
                em = false;
            }
        }
        if (em)
        {
            if (string.IsNullOrEmpty(email))
            {
                DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                    "ERROR!",
                    "Email can not be empty.",
                    DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                    "OK");
                return;
            }
            if (!Encryption.IsEmailValid(email))
            {
                DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                    "ERROR!",
                    "Email is not valid.",
                    DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                    "OK");
                return;
            }
            DevelopersHub.HTTPNetworking.Tools.Loading.Show();
            Authentication.OnPasswordRecoveryCodeSentSuccessful += Authentication_OnPasswordRecoveryCodeSentSuccessful;
            Authentication.OnPasswordRecoveryCodeSendFailed += Authentication_OnPasswordRecoveryCodeSendFailed;
            Authentication.SendPasswordRecoveryCode(email);
        }
        else
        {
            if (string.IsNullOrEmpty(phoneNumer))
            {
                DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                    "ERROR!",
                    "Phone number can not be empty.",
                    DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                    "OK");
                return;
            }
            if (!Encryption.IsPhoneNumberValid(phoneNumer, phoneCountry))
            {
                DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                    "ERROR!",
                    "Phone number is not valid.",
                    DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                    "OK");
                return;
            }
            DevelopersHub.HTTPNetworking.Tools.Loading.Show();
            Authentication.OnPasswordRecoveryCodeSentSuccessful += Authentication_OnPasswordRecoveryCodeSentSuccessful;
            Authentication.OnPasswordRecoveryCodeSendFailed += Authentication_OnPasswordRecoveryCodeSendFailed;
            Authentication.SendPasswordRecoveryCode(phoneNumer, phoneCountry);
        }
    }

    private void Authentication_OnPasswordRecoveryCodeSendFailed(string error)
    {
        Authentication.OnPasswordRecoveryCodeSentSuccessful -= Authentication_OnPasswordRecoveryCodeSentSuccessful;
        Authentication.OnPhoneVerificationCodeSendFailed -= Authentication_OnPasswordRecoveryCodeSendFailed;
        codeLife = 0;
        if (error == "ALREADY_SENT")
        {
            if (recoverPortraitEmail.isOn)
            {
                changePassPortraitAddress.text = "Sent to " + email;
                changePassLandscapeAddress.text = changePassPortraitAddress.text;
            }
            else
            {
                changePassPortraitAddress.text = "Sent to " + phoneNumer;
                changePassLandscapeAddress.text = changePassPortraitAddress.text;
            }
            changePassPortraitTime.text = "";
            changePassLandscapeTime.text = "";
            OpenPage(Page.password);
        }
        else
        {
            string er = "Failed to send recovery code.";
            switch (error)
            {
                case "USER_NOT_EXISTS":
                    er = "There is no user with the data that you entered.";
                    break;
            }
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
               "ERROR!",
               er,
               DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
               "OK");
        }
        DevelopersHub.HTTPNetworking.Tools.Loading.Hide();
    }

    private void Authentication_OnPasswordRecoveryCodeSentSuccessful(int remainedTime)
    {
        Authentication.OnPasswordRecoveryCodeSentSuccessful -= Authentication_OnPasswordRecoveryCodeSentSuccessful;
        Authentication.OnPhoneVerificationCodeSendFailed -= Authentication_OnPasswordRecoveryCodeSendFailed;
        codeLife = remainedTime;
        if (recoverPortraitEmail.isOn)
        {
            changePassPortraitAddress.text = "Sent to " + email;
            changePassLandscapeAddress.text = changePassPortraitAddress.text;
        }
        else
        {
            changePassPortraitAddress.text = "Sent to " + phoneNumer;
            changePassLandscapeAddress.text = changePassPortraitAddress.text;
        }
        changePassPortraitTime.text = "";
        changePassLandscapeTime.text = "";
        OpenPage(Page.password);
        MessageBox.ShowMessage("SUCCESS!", "Code sent successfully. You can use that to change your account's password.", MessageBox.Type.success, "OK");
    }

    private void SignUpCheck()
    {
        if (Screen.width < Screen.height)
        {
            password = signupPortraitPassword.text.Trim();
            username = signupPortraitUsername.text.Trim();
            email = signupPortraitEmail.text.Trim();
        }
        else
        {
            password = signupLandscapePassword.text.Trim();
            username = signupLandscapeUsername.text.Trim();
            email = signupLandscapeEmail.text.Trim();
        }
        if (string.IsNullOrEmpty(username))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Username can not be empty.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }
        if (string.IsNullOrEmpty(email))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Email can not be empty.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }
        if (!Encryption.IsEmailValid(email))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Email is not valid.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Password can not be empty.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }
        if (Networking.IsConnectedToServer)
        {
            SignUpDo();
        }
        else
        {
            signingUp = true;
            signingIn = false;
            DevelopersHub.HTTPNetworking.Tools.Loading.Show();
            Networking.ConnectToServer();
        }
    }

    private void SignUpDo()
    {
        signingUp = true;
        signingIn = false;
        Authentication.OnAuthenticationSuccessful += Authentication_OnAuthenticationSuccessful;
        Authentication.OnAuthenticationFailed += Authentication_OnAuthenticationFailed;
        DevelopersHub.HTTPNetworking.Tools.Loading.Show();
        Authentication.Register(username, password, email);
    }

    private void SignInCheck()
    {
        if (Screen.width < Screen.height)
        {
            password = signinPortraitPassword.text.Trim();
            username = signinPortraitUsername.text.Trim();
        }
        else
        {
            password = signinLandscapePassword.text.Trim();
            username = signinLandscapeUsername.text.Trim();
        }
        if (string.IsNullOrEmpty(username))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!", 
                "Username can not be empty.", 
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error, 
                "OK");
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
                "ERROR!",
                "Password can not be empty.",
                DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
                "OK");
            return;
        }
        if (Networking.IsConnectedToServer)
        {
            SignInDo();
        }
        else
        {
            signingIn = true;
            signingUp = false;
            DevelopersHub.HTTPNetworking.Tools.Loading.Show();
            Networking.ConnectToServer();
        }
    }

    private void SignInDo()
    {
        signingIn = true;
        signingUp = false;
        Authentication.OnAuthenticationSuccessful += Authentication_OnAuthenticationSuccessful;
        Authentication.OnAuthenticationFailed += Authentication_OnAuthenticationFailed;   
        DevelopersHub.HTTPNetworking.Tools.Loading.Show();
        Authentication.Authenticate(username, password, false);
    }

    private void Authentication_OnAuthenticationFailed(string error)
    {
        Authentication.OnAuthenticationSuccessful -= Authentication_OnAuthenticationSuccessful;
        Authentication.OnAuthenticationFailed -= Authentication_OnAuthenticationFailed;
        DevelopersHub.HTTPNetworking.Tools.Loading.Hide();
        string er = "Failed to create account.";
        switch (error)
        {
            case "ERROR_USERNAME_LENGHT":
                er = "Username must be minimum " + Authentication.Instance.UsernameMinLenght.ToString() + " and maximum " + Authentication.Instance.UsernameMaxLenght.ToString() + " characters.";
                break;
            case "ERROR_PASSWORD_LENGHT":
                er = "Password must be minimum " + Authentication.Instance.PasswordMinLenght.ToString() + " and maximum " + Authentication.Instance.PasswordMaxLenght.ToString() + " characters.";
                break;
            case "WRONG_CREDENTIALS":
                er = "Username or password is wrong.";
                break;
            case "USERNAME_TAKEN":
                er = "This username is not available. Please enter another username and try again.";
                break;
        }
        DevelopersHub.HTTPNetworking.Tools.MessageBox.ShowMessage(
           "ERROR!",
           er,
           DevelopersHub.HTTPNetworking.Tools.MessageBox.Type.error,
           "OK");
        signingIn = false;
        signingUp = false;
    }

    private void Authentication_OnAuthenticationSuccessful()
    {
        Authentication.OnAuthenticationSuccessful -= Authentication_OnAuthenticationSuccessful;
        Authentication.OnAuthenticationFailed -= Authentication_OnAuthenticationFailed;
        GoToAccountScene();
        signingIn = false;
        signingUp = false;
    }

    private void SignInForgotten()
    {
        OpenPage(Page.recovery);
    }

    private void OnRecoverCountryChanged(int value)
    {
        string code = countryCodes.Values.ElementAt(value);
        recoverPortraitCountryCode.text = code;
        recoverLandscapeCountryCode.text = code;
    }

    private void OnRecoverWithEmail(bool value)
    {
        if (value)
        {
            emailRecoverPortrait.text = "";
            emailRecoverLandscape.text = "";
            EmailPhoneSwitch();
        }
    }

    private void OnRecoverWithPhone(bool value)
    {
        if (value)
        {
            phoneRecoverPortrait.text = "";
            phoneRecoverLandscape.text = "";
            countryRecoverPortrait.value = 0;
            countryRecoverLandscape.value = 0;
            EmailPhoneSwitch();
        }
    }

    private void EmailPhoneSwitch()
    {
        emailRecoverPortrait.gameObject.SetActive(recoverPortraitEmail.isOn);
        emailRecoverLandscape.gameObject.SetActive(recoverLandscapeEmail.isOn);
        phoneRecoverPortrait.gameObject.SetActive(recoverPortraitPhone.isOn);
        phoneRecoverLandscape.gameObject.SetActive(recoverLandscapePhone.isOn);
        countryRecoverPortrait.gameObject.SetActive(recoverPortraitPhone.isOn);
        countryRecoverLandscape.gameObject.SetActive(recoverLandscapePhone.isOn);
    }

    private string GetCountryCode(string country)
    {
        string code = "+1";
        countryCodes.TryGetValue(country.ToUpper(), out code);
        return code;
    }

    private void SetCountryCode(Dropdown dropdown)
    {
        List<string> countries = countryCodes.Select(kvp => kvp.Key).ToList();
        dropdown.ClearOptions();
        dropdown.AddOptions(countries);
        dropdown.SetValueWithoutNotify(0);
    }

    private void OpenPage(Page p)
    {
        landing.gameObject.SetActive(p == Page.landing);
        signin.gameObject.SetActive(p == Page.signin);
        signup.gameObject.SetActive(p == Page.signup);
        recovery.gameObject.SetActive(p == Page.recovery);
        changePass.gameObject.SetActive(p == Page.password);
        switch (p)
        {
            case Page.landing:

                break;
            case Page.signin:
                signinPortraitPassword.text = "";
                signinPortraitUsername.text = "";
                signinLandscapePassword.text = "";
                signinLandscapeUsername.text = "";
                break;
            case Page.signup:
                signupPortraitEmail.text = "";
                signupPortraitUsername.text = "";
                signupPortraitPassword.text = "";
                signupLandscapeEmail.text = "";
                signupLandscapeUsername.text = "";
                signupLandscapePassword.text = "";
                break;
            case Page.recovery:
                emailRecoverPortrait.text = "";
                emailRecoverLandscape.text = "";
                phoneRecoverPortrait.text = "";
                phoneRecoverLandscape.text = "";
                countryRecoverPortrait.SetValueWithoutNotify(0);
                countryRecoverLandscape.SetValueWithoutNotify(0);
                OnRecoverCountryChanged(0);
                break;
            case Page.password:
                changePassPortraitCode.text = "";
                changePassLandscapeCode.text = "";
                changePassPortraitPass.text = "";
                changePassLandscapePass.text = "";
                break;
        }
        page = p;
    }

    private void Back()
    {

        switch (page)
        {
            case Page.landing:

                break;
            case Page.signin:
                OpenPage(Page.landing);
                break;
            case Page.signup:
                OpenPage(Page.landing);
                break;
            case Page.recovery:
                OpenPage(Page.signin);
                break;
            case Page.password:
                OpenPage(Page.recovery);
                break;
        }
    }

    private void OnScreenSizeChanged()
    {
        bool portrait = (Screen.width < Screen.height);
        landingPortrait.gameObject.SetActive(portrait);
        landingLandscape.gameObject.SetActive(!portrait);
        signinPortrait.gameObject.SetActive(portrait);
        signinLandscape.gameObject.SetActive(!portrait);
        signupPortrait.gameObject.SetActive(portrait);
        signupLandscape.gameObject.SetActive(!portrait);
        recoverPortrait.gameObject.SetActive(portrait);
        recoverLandscape.gameObject.SetActive(!portrait);
        changePassPortrait.gameObject.SetActive(portrait);
        changePassLandscape.gameObject.SetActive(!portrait);
        if (portrait && !lastPortrait)
        {
            signinPortraitUsername.text = signinLandscapeUsername.text;
            signinPortraitPassword.text = signinLandscapePassword.text;
            signupPortraitUsername.text = signupLandscapeUsername.text;
            signupPortraitPassword.text = signupLandscapePassword.text;
            signupPortraitEmail.text = signupLandscapeEmail.text;
            emailRecoverPortrait.text = emailRecoverLandscape.text;
            changePassPortraitCode.text = changePassLandscapeCode.text;
            changePassPortraitPass.text = changePassLandscapePass.text;
            phoneRecoverPortrait.text = phoneRecoverLandscape.text;
            countryRecoverPortrait.SetValueWithoutNotify(countryRecoverLandscape.value);
            recoverPortraitEmail.isOn = recoverLandscapeEmail.isOn;
            recoverPortraitPhone.isOn = recoverLandscapePhone.isOn;
            recoverPortraitCountryCode.text = recoverLandscapeCountryCode.text;
        }
        else if(!portrait && lastPortrait)
        {
            signinLandscapeUsername.text = signinPortraitUsername.text;
            signinLandscapePassword.text = signinPortraitPassword.text;
            signupLandscapeUsername.text = signupPortraitUsername.text;
            signupLandscapePassword.text = signupPortraitPassword.text;
            signupLandscapeEmail.text = signupPortraitEmail.text;
            emailRecoverLandscape.text = emailRecoverPortrait.text;
            changePassLandscapeCode.text = changePassPortraitCode.text;
            changePassLandscapePass.text = changePassPortraitPass.text;
            phoneRecoverLandscape.text = phoneRecoverPortrait.text;
            countryRecoverLandscape.SetValueWithoutNotify(countryRecoverPortrait.value);
            recoverLandscapeEmail.isOn = recoverPortraitEmail.isOn;
            recoverLandscapePhone.isOn = recoverPortraitPhone.isOn;
            recoverLandscapeCountryCode.text = recoverPortraitCountryCode.text;
        }
        lastPortrait = portrait;
    }

    private void MessageBoxButton1_Clicked()
    {


    }

    private void MessageBoxButton2_Clicked()
    {


    }

    #region Network Callbacks
    private void HTTPNetworking_OnConnectedToServerSuccessful()
    {
        if (signingIn)
        {
            SignInDo();
        }
        else if (signingUp)
        {
            SignUpDo();
        }
    }

    private void HTTPNetworking_OnConnectToServerFailed(string error)
    {
        DevelopersHub.HTTPNetworking.Tools.Loading.Hide();
        if (signingIn)
        {
            Authentication.OnAuthenticationSuccessful -= Authentication_OnAuthenticationSuccessful;
            Authentication.OnAuthenticationFailed -= Authentication_OnAuthenticationFailed;
            signingIn = false;
        }
        else if (signingUp)
        {
            Authentication.OnAuthenticationSuccessful -= Authentication_OnAuthenticationSuccessful;
            Authentication.OnAuthenticationFailed -= Authentication_OnAuthenticationFailed;
            signingUp = false;
        }
        MessageBox.ShowMessage("ERROR!", "Failed to connect to the server. Please check your internet connection.", MessageBox.Type.error, "OK");
    }
    
    private void HTTPNetworking_OnDisconnectedFromServer(string error)
    {

    }
    #endregion

    private Dictionary<string, string> countryCodes = new Dictionary<string, string>
    {
    {"AC", "+247"},
    {"AD", "+376"},
    {"AE", "+971"},
    {"AF", "+93"},
    {"AG", "+1"},
    {"AI", "+1"},
    {"AL", "+355"},
    {"AM", "+374"},
    {"AN", "+599"},
    {"AO", "+244"},
    {"AR", "+54"},
    {"AS", "+1"},
    {"AT", "+43"},
    {"AU", "+61"},
    {"AW", "+297"},
    {"AX", "+358"},
    {"AZ", "+994"},
    {"BA", "+387"},
    {"BB", "+1"},
    {"BD", "+880"},
    {"BE", "+32"},
    {"BF", "+226"},
    {"BG", "+359"},
    {"BH", "+973"},
    {"BI", "+257"},
    {"BJ", "+229"},
    {"BM", "+1"},
    {"BN", "+673"},
    {"BO", "+591"},
    {"BR", "+55"},
    {"BS", "+1"},
    {"BT", "+975"},
    {"BW", "+267"},
    {"BY", "+375"},
    {"BZ", "+501"},
    {"CA", "+1"},
    {"CC", "+61"},
    {"CD", "+243"},
    {"CF", "+236"},
    {"CG", "+242"},
    {"CH", "+41"},
    {"CI", "+225"},
    {"CK", "+682"},
    {"CL", "+56"},
    {"CM", "+237"},
    {"CN", "+86"},
    {"CO", "+57"},
    {"CR", "+506"},
    {"CS", "+381"},
    {"CU", "+53"},
    {"CV", "+238"},
    {"CX", "+61"},
    {"CY", "+357"},
    {"CZ", "+420"},
    {"DE", "+49"},
    {"DJ", "+253"},
    {"DK", "+45"},
    {"DM", "+1"},
    {"DO", "+1"},
    {"DZ", "+213"},
    {"EC", "+593"},
    {"EE", "+372"},
    {"EG", "+20"},
    {"EH", "+212"},
    {"ER", "+291"},
    {"ES", "+34"},
    {"ET", "+251"},
    {"FI", "+358"},
    {"FJ", "+679"},
    {"FK", "+500"},
    {"FM", "+691"},
    {"FO", "+298"},
    {"FR", "+33"},
    {"GA", "+241"},
    {"GB", "+44"},
    {"GD", "+1"},
    {"GE", "+995"},
    {"GF", "+594"},
    {"GG", "+44"},
    {"GH", "+233"},
    {"GI", "+350"},
    {"GL", "+299"},
    {"GM", "+220"},
    {"GN", "+224"},
    {"GP", "+590"},
    {"GQ", "+240"},
    {"GR", "+30"},
    {"GT", "+502"},
    {"GU", "+1"},
    {"GW", "+245"},
    {"GY", "+592"},
    {"HK", "+852"},
    {"HN", "+504"},
    {"HR", "+385"},
    {"HT", "+509"},
    {"HU", "+36"},
    {"ID", "+62"},
    {"IE", "+353"},
    {"IL", "+972"},
    {"IM", "+44"},
    {"IN", "+91"},
    {"IO", "+246"},
    {"IQ", "+964"},
    {"IR", "+98"},
    {"IS", "+354"},
    {"IT", "+39"},
    {"JE", "+44"},
    {"JM", "+1"},
    {"JO", "+962"},
    {"JP", "+81"},
    {"KE", "+254"},
    {"KG", "+996"},
    {"KH", "+855"},
    {"KI", "+686"},
    {"KM", "+269"},
    {"KN", "+1"},
    {"KP", "+850"},
    {"KR", "+82"},
    {"KW", "+965"},
    {"KY", "+1"},
    {"KZ", "+7"},
    {"LA", "+856"},
    {"LB", "+961"},
    {"LC", "+1"},
    {"LI", "+423"},
    {"LK", "+94"},
    {"LR", "+231"},
    {"LS", "+266"},
    {"LT", "+370"},
    {"LU", "+352"},
    {"LV", "+371"},
    {"LY", "+218"},
    {"MA", "+212"},
    {"MC", "+377"},
    {"MD", "+373"},
    {"ME", "+382"},
    {"MG", "+261"},
    {"MH", "+692"},
    {"MK", "+389"},
    {"ML", "+223"},
    {"MM", "+95"},
    {"MN", "+976"},
    {"MO", "+853"},
    {"MP", "+1"},
    {"MQ", "+596"},
    {"MR", "+222"},
    {"MS", "+1"},
    {"MT", "+356"},
    {"MU", "+230"},
    {"MV", "+960"},
    {"MW", "+265"},
    {"MX", "+52"},
    {"MY", "+60"},
    {"MZ", "+258"},
    {"NA", "+264"},
    {"NC", "+687"},
    {"NE", "+227"},
    {"NF", "+672"},
    {"NG", "+234"},
    {"NI", "+505"},
    {"NL", "+31"},
    {"NO", "+47"},
    {"NP", "+977"},
    {"NR", "+674"},
    {"NU", "+683"},
    {"NZ", "+64"},
    {"OM", "+968"},
    {"PA", "+507"},
    {"PE", "+51"},
    {"PF", "+689"},
    {"PG", "+675"},
    {"PH", "+63"},
    {"PK", "+92"},
    {"PL", "+48"},
    {"PM", "+508"},
    {"PR", "+1"},
    {"PS", "+970"},
    {"PT", "+351"},
    {"PW", "+680"},
    {"PY", "+595"},
    {"QA", "+974"},
    {"RE", "+262"},
    {"RO", "+40"},
    {"RS", "+381"},
    {"RU", "+7"},
    {"RW", "+250"},
    {"SA", "+966"},
    {"SB", "+677"},
    {"SC", "+248"},
    {"SD", "+249"},
    {"SE", "+46"},
    {"SG", "+65"},
    {"SH", "+290"},
    {"SI", "+386"},
    {"SJ", "+47"},
    {"SK", "+421"},
    {"SL", "+232"},
    {"SM", "+378"},
    {"SN", "+221"},
    {"SO", "+252"},
    {"SR", "+597"},
    {"ST", "+239"},
    {"SV", "+503"},
    {"SY", "+963"},
    {"SZ", "+268"},
    {"TA", "+290"},
    {"TC", "+1"},
    {"TD", "+235"},
    {"TG", "+228"},
    {"TH", "+66"},
    {"TJ", "+992"},
    {"TK", "+690"},
    {"TL", "+670"},
    {"TM", "+993"},
    {"TN", "+216"},
    {"TO", "+676"},
    {"TR", "+90"},
    {"TT", "+1"},
    {"TV", "+688"},
    {"TW", "+886"},
    {"TZ", "+255"},
    {"UA", "+380"},
    {"UG", "+256"},
    {"US", "+1"},
    {"UY", "+598"},
    {"UZ", "+998"},
    {"VA", "+379"},
    {"VC", "+1"},
    {"VE", "+58"},
    {"VG", "+1"},
    {"VI", "+1"},
    {"VN", "+84"},
    {"VU", "+678"},
    {"WF", "+681"},
    {"WS", "+685"},
    {"YE", "+967"},
    {"YT", "+262"},
    {"ZA", "+27"},
    {"ZM", "+260"},
    {"ZW", "+263"}
    };

}
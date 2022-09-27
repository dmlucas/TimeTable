namespace DevelopersHub.HTTPNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using LitJson;
    using UnityEngine;
    using System;

    /// <summary>
    /// Identifies the player with a username and password.
    /// </summary>
    public class Authentication : Extension
    {

        #region Events
        public static event Networking.NoCallback OnAuthenticationSuccessful;
        public static event Networking.ErrorCallback OnAuthenticationFailed;

        public static event Networking.JsonDataCallback OnGetUserDataSuccessful;
        public static event Networking.ErrorCallback OnGetUserDataFailed;

        public static event UsersListCallback OnGetUsersDataPerPageSuccessful;
        public static event Networking.ErrorCallback OnGetUsersDataPerPageFailed;

        public static event VerificationCodeCallback OnEmailVerificationCodeSentSuccessful;
        public static event Networking.ErrorCallback OnEmailVerificationCodeSendFailed;

        public static event VerificationCodeCallback OnPhoneVerificationCodeSentSuccessful;
        public static event Networking.ErrorCallback OnPhoneVerificationCodeSendFailed;

        public static event Networking.UsernameCallback OnChangePasswordSuccessful;
        public static event Networking.ErrorCallback OnChangePasswordFailed;

        public static event Networking.NoCallback OnChangeEmailSuccessful;
        public static event Networking.ErrorCallback OnChangeEmailFailed;

        public static event Networking.NoCallback OnChangePhoneNumberSuccessful;
        public static event Networking.ErrorCallback OnChangePhoneNumberFailed;

        public static event Networking.NoCallback OnVerifyEmailSuccessful;
        public static event Networking.ErrorCallback OnVerifyEmailFailed;

        public static event Networking.NoCallback OnVerifyPhoneNumberSuccessful;
        public static event Networking.ErrorCallback OnVerifyPhoneNumberFailed;

        public static event Networking.RemainedTimeCallback OnPasswordRecoveryCodeSentSuccessful;
        public static event Networking.ErrorCallback OnPasswordRecoveryCodeSendFailed;

        public static event Networking.UsernameCallback OnUsernameChangedSuccessful;
        public static event Networking.ErrorCallback OnUsernameChangeFailed;
        #endregion

        #region Core
        [Header("Settings")]
        [SerializeField] [Range(3, 50)] private int usernameMaxLenght = 20; public int UsernameMaxLenght { get { return usernameMaxLenght; } }
        [SerializeField] [Range(3, 50)] private int usernameMinLenght = 3; public int UsernameMinLenght { get { return usernameMinLenght; } }
        [SerializeField] [Range(1, 50)] private int passwordMaxLenght = 20; public int PasswordMaxLenght { get { return passwordMaxLenght; } }
        [SerializeField] [Range(1, 50)] private int passwordMinLenght = 6; public int PasswordMinLenght { get { return passwordMinLenght; } }
        private bool _authenticated = false; public static bool IsAuthenticated { get { return instance ? instance._authenticated : false; } }
        private string _username = ""; public static string Username { get { return instance ? instance._username : ""; } }
        private int _id = 0; public static int UserID { get { return instance ? instance._id : 0; } }
        private string _password = ""; public static string Password { get { return instance ? instance._password : ""; } }
        private string _session = ""; public static string SessionToken { get { return instance ? instance._session : ""; } }
        private int _sessionId = 0; public static int SessionID { get { return instance ? instance._sessionId : 0; } }
        private bool _authenticateOnConnect = false;
        private bool _regiserIfNotExist = false;
        private bool _regiserOnly = false;
        private string _registerEmail = "";
        private string _registerPhoneNumber = "";
        private string _registerPhoneCountry = "";
        private string _registerBirthday = "";
        private string _registerFirstname = "";
        private string _registerLastname = "";
        private static string _usernameKey = "pfx_default_username_key_xyz";
        private static string _passwordKey = "pfx_default_password_key_xyz";
        private static string _ivKey = "pfx_default_iv_key_xyz";
        private static int AUTHENTICATE_USER = 987700;
        private static int UPDATE_USER_ACTIVITY = 987701;
        private static int GET_USER_DATA = 987702;
        private static int GET_USERS_DATA_PER_PAGE = 987703;
        private static int CHANGE_PASSWORD = 987704;
        private static int CHANGE_EMAIL = 987705;
        private static int SEND_EMAIL_VERIFICATION_CODE = 987706;
        private static int CHANGE_PHONE = 987707;
        private static int SEND_PHONE_VERIFICATION_CODE = 987708;
        private static int VERIFY_PHONE = 987709;
        private static int VERIFY_EMAIL = 987710;
        private static int SEND_PASSWORD_RECOVERY_CODE = 987711;
        private static int CHANGE_USERNAME = 987712;
        public delegate void UsersListCallback(JsonData users, int totalUsersCount, int currentPage, int countPerPage);
        public delegate void VerificationCodeCallback(int userID, int remained);
        public delegate void UsernameCallback(string username);
        private static Authentication instance = null; public static Authentication Instance
        {
            get
            {
                return instance;
            }
        }

        public static void Initialize()
        {
            if (instance && instance.initialized)
            {
                return;
            }
            if (instance == null)
            {
                instance = Networking.Instance.gameObject.AddComponent<Authentication>();
            }
            if (!Networking.Instance.Extensions.Contains(instance))
            {
                Networking.Instance.Extensions.Add(instance);
            }
            instance.initialized = true;
            Networking.OnDisconnectedFromServer += OnDisconnectedFromServer;
            Networking.OnConnectedToServerSuccessful += OnConnectedToServerSuccessful;
            Networking.OnConnectToServerFailed += OnConnectToServerFailed;
        }

        private static void OnConnectedToServerSuccessful()
        {
            if(instance._authenticateOnConnect)
            {
                instance._authenticateOnConnect = false;
                Authenticate();
            }
        }

        private static void OnConnectToServerFailed(string error)
        {
            if(instance._authenticateOnConnect)
            {
                instance._authenticateOnConnect = false;
                if (OnAuthenticationFailed != null)
                {
                    OnAuthenticationFailed.Invoke("ERROR_CONNECTION");
                }
            }
        }

        private static void OnDisconnectedFromServer(string error)
        {
            if (instance && instance._authenticated)
            {
                instance._authenticated = false;
                switch (error)
                {
                    default:

                        break;
                }
                instance.Clear();
            }
        }

        public static void Stop()
        {
            if (instance != null)
            {
                instance.initialized = false;
                instance._authenticated = false;
                instance.Clear();
                if (Networking.Instance.Extensions.Contains(instance))
                {
                    Networking.Instance.Extensions.Remove(instance);
                }
                Networking.OnDisconnectedFromServer -= OnDisconnectedFromServer;
                Networking.OnConnectedToServerSuccessful -= OnConnectedToServerSuccessful;
                Networking.OnConnectToServerFailed -= OnConnectToServerFailed;
            }
        }

        public override HookData Hook()
        {
            HookData hook = new HookData();
            hook.data = new Dictionary<string, object>();
            List<int> requests = new List<int>();
            Hook(ref hook.data, ref requests);
            if (requests.Count > 0)
            {
                hook.requests = new int[requests.Count];
                _requests = new int[requests.Count];
                for (int i = 0; i < requests.Count; i++)
                {
                    hook.requests[i] = (int)requests[i];
                    _requests[i] = hook.requests[i];
                }
            }
            return hook;
        }

        private int[] _requests = null;

        public override void Sync(JsonData data)
        {
            if (_requests == null || _requests.Length <= 0 || data == null)
            {
                return;
            }
            for (int i = 0; i < _requests.Length; i++)
            {
                if (data.ContainsKey(_requests[i].ToString()))
                {
                    ProcessResponse(_requests[i], data[_requests[i].ToString()]);
                }
            }
        }

        public override void ProcessCoreRequest(int requestID, bool successful, string error, JsonData data)
        {
            if (requestID == AUTHENTICATE_USER)
            {
                if (successful)
                {
                    _authenticated = true;
                    string iv = Encryption.GenerateVI(16);
                    PlayerPrefs.SetString(_ivKey, iv);
                    PlayerPrefs.SetString(_usernameKey, Encryption.EncryptAES(_username, iv, Networking.Instance.settings.EncryptionKey));
                    PlayerPrefs.SetString(_passwordKey, Encryption.EncryptAES(_password, iv, Networking.Instance.settings.EncryptionKey));
                    if (data.ContainsKey("account_id"))
                    {
                        _id = int.Parse(data["account_id"].ToString());
                    }
                    if (data.ContainsKey("session_id"))
                    {
                        _sessionId = int.Parse(data["session_id"].ToString());
                    }
                    if (OnAuthenticationSuccessful != null)
                    {
                        OnAuthenticationSuccessful.Invoke();
                    }
                }
                else
                {
                    if (OnAuthenticationFailed != null)
                    {
                        OnAuthenticationFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == GET_USER_DATA)
            {
                if(successful && data != null && data.ContainsKey("user"))
                {
                    if (OnGetUserDataSuccessful != null)
                    {
                        OnGetUserDataSuccessful.Invoke(data["user"]);
                    }
                }
                else
                {
                    if (OnGetUserDataFailed != null)
                    {
                        OnGetUserDataFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == GET_USERS_DATA_PER_PAGE)
            {
                if (successful && data != null && data.ContainsKey("users"))
                {
                    int count = 0;
                    int page = 1;
                    int perPage = 10;
                    if (data.ContainsKey("total_users_count"))
                    {
                        int.TryParse(data["total_users_count"].ToString(), out count);
                    }
                    if (data.ContainsKey("per_page_count"))
                    {
                        int.TryParse(data["per_page_count"].ToString(), out perPage);
                    }
                    if (data.ContainsKey("current_page"))
                    {
                        int.TryParse(data["current_page"].ToString(), out page);
                    }
                    if (OnGetUsersDataPerPageSuccessful != null)
                    {
                        OnGetUsersDataPerPageSuccessful.Invoke(data["users"], count, page, perPage);
                    }
                }
                else
                {
                    if (OnGetUsersDataPerPageFailed != null)
                    {
                        OnGetUsersDataPerPageFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == SEND_EMAIL_VERIFICATION_CODE)
            {
                if (successful && data != null && data.ContainsKey("remained") && data.ContainsKey("id"))
                {
                    if (OnEmailVerificationCodeSentSuccessful != null)
                    {
                        OnEmailVerificationCodeSentSuccessful.Invoke(int.Parse(data["id"].ToString()), int.Parse(data["remained"].ToString()));
                    }
                }
                else
                {
                    if (OnEmailVerificationCodeSendFailed != null)
                    {
                        OnEmailVerificationCodeSendFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == SEND_PHONE_VERIFICATION_CODE)
            {
                if (successful && data != null && data.ContainsKey("remained") && data.ContainsKey("id"))
                {
                    if (OnPhoneVerificationCodeSentSuccessful != null)
                    {
                        OnPhoneVerificationCodeSentSuccessful.Invoke(int.Parse(data["id"].ToString()), int.Parse(data["remained"].ToString()));
                    }
                }
                else
                {
                    if (OnPhoneVerificationCodeSendFailed != null)
                    {
                        OnPhoneVerificationCodeSendFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == CHANGE_PASSWORD)
            {
                if (successful && data != null && data.ContainsKey("new_password") && data.ContainsKey("username"))
                {
                    _password = data["new_password"].ToString();
                    string user = data["username"].ToString();
                    if (instance._authenticated && user == _username)
                    {
                        string iv = "null";
                        if (PlayerPrefs.HasKey(_ivKey))
                        {
                            iv = PlayerPrefs.GetString(_ivKey);
                        }
                        else
                        {
                            iv = Encryption.GenerateVI(16);
                            PlayerPrefs.SetString(_ivKey, iv);
                        }
                        PlayerPrefs.SetString(_passwordKey, Encryption.EncryptAES(_password, iv, Networking.Instance.settings.EncryptionKey));
                    }
                    if (OnChangePasswordSuccessful != null)
                    {
                        OnChangePasswordSuccessful.Invoke(user);
                    }
                }
                else
                {
                    if (OnChangePasswordFailed != null)
                    {
                        OnChangePasswordFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == CHANGE_PHONE)
            {
                if (successful)
                {
                    if (OnChangePhoneNumberSuccessful != null)
                    {
                        OnChangePhoneNumberSuccessful.Invoke();
                    }
                }
                else
                {
                    if (OnChangePhoneNumberFailed != null)
                    {
                        OnChangePhoneNumberFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == CHANGE_EMAIL)
            {
                if (successful)
                {
                    if (OnChangeEmailSuccessful != null)
                    {
                        OnChangeEmailSuccessful.Invoke();
                    }
                }
                else
                {
                    if (OnChangeEmailFailed != null)
                    {
                        OnChangeEmailFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == VERIFY_PHONE)
            {
                if (successful)
                {
                    if (OnVerifyPhoneNumberSuccessful != null)
                    {
                        OnVerifyPhoneNumberSuccessful.Invoke();
                    }
                }
                else
                {
                    if (OnVerifyPhoneNumberFailed != null)
                    {
                        OnVerifyPhoneNumberFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == VERIFY_EMAIL)
            {
                if (successful)
                {
                    if (OnVerifyEmailSuccessful != null)
                    {
                        OnVerifyEmailSuccessful.Invoke();
                    }
                }
                else
                {
                    if (OnVerifyEmailFailed != null)
                    {
                        OnVerifyEmailFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == SEND_PASSWORD_RECOVERY_CODE)
            {
                if (successful)
                {
                    if (OnPasswordRecoveryCodeSentSuccessful != null && data.ContainsKey("remained"))
                    {
                        OnPasswordRecoveryCodeSentSuccessful.Invoke(int.Parse(data["remained"].ToString()));
                    }
                }
                else
                {
                    if (OnPasswordRecoveryCodeSendFailed != null)
                    {
                        OnPasswordRecoveryCodeSendFailed.Invoke(error);
                    }
                }
            }
            else if (requestID == CHANGE_USERNAME)
            {
                if (successful)
                {
                    if (OnUsernameChangedSuccessful != null && data.ContainsKey("username"))
                    {
                        _username = data["username"].ToString();
                        if (instance._authenticated)
                        {
                            string iv = "null";
                            if (PlayerPrefs.HasKey(_ivKey))
                            {
                                iv = PlayerPrefs.GetString(_ivKey);
                            }
                            else
                            {
                                iv = Encryption.GenerateVI(16);
                                PlayerPrefs.SetString(_ivKey, iv);
                            }
                            PlayerPrefs.SetString(_usernameKey, Encryption.EncryptAES(_username, iv, Networking.Instance.settings.EncryptionKey));
                        }
                        OnUsernameChangedSuccessful.Invoke(_username);
                    }
                }
                else
                {
                    if (OnUsernameChangeFailed != null)
                    {
                        OnUsernameChangeFailed.Invoke(error);
                    }
                }
            }
        }

        public static void Authenticate(bool regiserIfNotExist)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            instance._regiserIfNotExist = regiserIfNotExist;
            bool found = false;
            if (PlayerPrefs.HasKey(_usernameKey) && PlayerPrefs.HasKey(_passwordKey) && PlayerPrefs.HasKey(_ivKey))
            {
                try
                {
                    string iv = PlayerPrefs.GetString(_ivKey);
                    instance._username = Encryption.DecryptAES(PlayerPrefs.GetString(_usernameKey), iv, Networking.Instance.settings.EncryptionKey);
                    instance._password = Encryption.DecryptAES(PlayerPrefs.GetString(_passwordKey), iv, Networking.Instance.settings.EncryptionKey);
                    found = true;
                }
                catch (Exception)
                {

                }
            }
            if(!found)
            {
                if (regiserIfNotExist)
                {
                    instance._username = GetRandomUsername();
                    instance._password = Encryption.EncryptMD5(GetRandomString(UnityEngine.Random.Range(instance.PasswordMinLenght, instance.PasswordMaxLenght + 1)));
                }
                else
                {
                    if (OnAuthenticationFailed != null)
                    {
                        OnAuthenticationFailed.Invoke("NO_CREDENTIALS");
                    }
                    return;
                }
            }
            instance._registerBirthday = null;
            instance._registerFirstname = null;
            instance._registerLastname = null;
            instance._registerEmail = null;
            instance._registerPhoneNumber = null;
            instance._registerPhoneCountry = null;
            instance._regiserOnly = false;
            instance._session = GetRandomString(UnityEngine.Random.Range(15, 25));
            Authenticate();
        }

        public static void Authenticate(string username, string password, bool regiserIfNotExist, string email = "", string phoneNumber = "", string phoneCountry = "", string firstName = "", string lastName = "", string birthday = "")
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            instance._regiserIfNotExist = regiserIfNotExist;
            if (username.Length < instance.UsernameMinLenght || username.Length > instance.UsernameMaxLenght)
            {
                if (OnAuthenticationFailed != null)
                {
                    OnAuthenticationFailed.Invoke("ERROR_USERNAME_LENGHT");
                }
            }
            else if (password.Length < instance.PasswordMinLenght || password.Length > instance.PasswordMaxLenght)
            {
                if (OnAuthenticationFailed != null)
                {
                    OnAuthenticationFailed.Invoke("ERROR_PASSWORD_LENGHT");
                }
            }
            else
            {
                instance._registerBirthday = birthday;
                instance._registerFirstname = firstName;
                instance._registerLastname = lastName;
                instance._registerEmail = email;
                instance._registerPhoneNumber = phoneNumber;
                instance._registerPhoneCountry = phoneCountry;
                instance._regiserOnly = false;
                instance._username = username;
                instance._password = Encryption.EncryptMD5(password);
                instance._session = GetRandomString(UnityEngine.Random.Range(15, 25));
                Authenticate();
            }
        }

        public static void Register(string username, string password, string email = "", string phoneNumber = "", string phoneCountry = "", string firstName = "", string lastName = "", string birthday = "")
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (username.Length < instance.UsernameMinLenght || username.Length > instance.UsernameMaxLenght)
            {
                if (OnAuthenticationFailed != null)
                {
                    OnAuthenticationFailed.Invoke("ERROR_USERNAME_LENGHT");
                }
            }
            else if (password.Length < instance.PasswordMinLenght || password.Length > instance.PasswordMaxLenght)
            {
                if (OnAuthenticationFailed != null)
                {
                    OnAuthenticationFailed.Invoke("ERROR_PASSWORD_LENGHT");
                }
            }
            else
            {
                instance._regiserIfNotExist = true;
                instance._registerBirthday = birthday;
                instance._registerFirstname = firstName;
                instance._registerLastname = lastName;
                instance._registerEmail = email;
                instance._registerPhoneNumber = phoneNumber;
                instance._registerPhoneCountry = phoneCountry;
                instance._regiserOnly = true;
                instance._username = username;
                instance._password = Encryption.EncryptMD5(password);
                instance._session = GetRandomString(UnityEngine.Random.Range(15, 25));
                Authenticate();
            }
        }

        private static void Authenticate()
        {   
            if (!Networking.IsConnectedToServer)
            {
                instance._authenticateOnConnect = true;
                Networking.ConnectToServer();
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("username", instance._username);
            data.Add("password", instance._password);
            data.Add("session", instance._session);
            data.Add("register_user", instance._regiserIfNotExist);
            data.Add("create_session", true);
            if (instance._regiserOnly)
            {
                data.Add("only_register", true);
            }
            if (!string.IsNullOrEmpty(instance._registerBirthday))
            {
                data.Add("birthday", instance._registerBirthday);
            }
            if (!string.IsNullOrEmpty(instance._registerEmail))
            {
                data.Add("email", instance._registerEmail);
            }
            if (!string.IsNullOrEmpty(instance._registerPhoneNumber))
            {
                data.Add("phone_number", instance._registerPhoneNumber);
            }
            if (!string.IsNullOrEmpty(instance._registerPhoneCountry))
            {
                data.Add("phone_country", instance._registerPhoneCountry);
            }
            if (!string.IsNullOrEmpty(instance._registerFirstname))
            {
                data.Add("firstname", instance._registerFirstname);
            }
            if (!string.IsNullOrEmpty(instance._registerLastname))
            {
                data.Add("lastname", instance._registerLastname);
            }
            Networking.SendRequest(AUTHENTICATE_USER, data);
        }

        public static void ChangeUsername(string username)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnUsernameChangeFailed != null)
                {
                    OnUsernameChangeFailed.Invoke("USERNAME_NOT_VALID");
                }
                return;
            }
            if (username.Length < instance.UsernameMinLenght || username.Length > instance.UsernameMaxLenght)
            {
                if (OnUsernameChangeFailed != null)
                {
                    OnUsernameChangeFailed.Invoke("ERROR_USERNAME_LENGHT");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("username", instance._username);
            data.Add("password", instance._password);
            data.Add("session", instance._session);
            data.Add("new_username", username);
            Networking.SendRequest(CHANGE_USERNAME, data);
        }

        public static void GetUserData(string username)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnGetUserDataFailed != null)
                {
                    OnGetUserDataFailed.Invoke("USER_NOT_EXISTS");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("get_username", username);
            if (instance._authenticated)
            {
                data.Add("your_id", UserID);
            }
            Networking.SendRequest(GET_USER_DATA, data);
        }

        public static void GetUserData(int id)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("get_id", id);
            if (instance._authenticated)
            {
                data.Add("your_id", UserID);
            }
            Networking.SendRequest(GET_USER_DATA, data);
        }

        public static void GetUsersDataPerPage(int page, int perPage = 10, Networking.SortType sortType = Networking.SortType.NONE, string sortColumn = "score")
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (page <= 0)
            {
                page = 1;
            }
            if (perPage <= 0)
            {
                perPage = 10;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("users_page", page);
            data.Add("users_per_page", perPage);
            data.Add("users_desc_asc", (int)sortType);
            data.Add("users_sort", sortColumn);
            if (instance._authenticated)
            {
                data.Add("your_id", UserID);
            }
            Networking.SendRequest(GET_USERS_DATA_PER_PAGE, data);
        }

        public static void SendEmailVerificationCode(string username)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnEmailVerificationCodeSendFailed != null)
                {
                    OnEmailVerificationCodeSendFailed.Invoke("USER_NOT_EXISTS");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("username", username);
            Networking.SendRequest(SEND_EMAIL_VERIFICATION_CODE, data);
        }

        public static void SendEmailVerificationCode(int id)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", id);
            Networking.SendRequest(SEND_EMAIL_VERIFICATION_CODE, data);
        }

        public static void SendPhoneVerificationCode(string username)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnPhoneVerificationCodeSendFailed != null)
                {
                    OnPhoneVerificationCodeSendFailed.Invoke("USER_NOT_EXISTS");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("username", username);
            Networking.SendRequest(SEND_PHONE_VERIFICATION_CODE, data);
        }

        public static void SendPhoneVerificationCode(int id)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", id);
            Networking.SendRequest(SEND_PHONE_VERIFICATION_CODE, data);
        }

        public static void ChangePassword(int id, string oldPassword, string newPassword)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("OLD_PASSWORD_BLANK");
                }
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("NEW_PASSWORD_BLANK");
                }
                return;
            }
            if (newPassword.Length < instance.PasswordMinLenght || newPassword.Length > instance.PasswordMaxLenght)
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("ERROR_PASSWORD_LENGHT");
                }
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", id);
            data.Add("old_password", Encryption.EncryptMD5(oldPassword));
            data.Add("new_password", Encryption.EncryptMD5(newPassword));
            Networking.SendRequest(CHANGE_PASSWORD, data);
        }

        public static void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("USER_NOT_EXISTS");
                }
                return;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("OLD_PASSWORD_BLANK");
                }
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("NEW_PASSWORD_BLANK");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("username", username);
            data.Add("old_password", Encryption.EncryptMD5(oldPassword));
            data.Add("new_password", Encryption.EncryptMD5(newPassword));
            Networking.SendRequest(CHANGE_PASSWORD, data);
        }

        public static void ChangePassword(string newPassword)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (!instance._authenticated)
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("NOT_AUTHENTICATED");
                }
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("NEW_PASSWORD_BLANK");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("new_password", Encryption.EncryptMD5(newPassword));
            data.Add("username", instance._username);
            data.Add("password", instance._password);
            data.Add("session", instance._session);
            Networking.SendRequest(CHANGE_PASSWORD, data);
        }

        public static void ChangePassword(string code, string newPassword, string email = null, string phone = null)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("EMAIL_OR_PHONE_NOT_VALID");
                }
                return;
            }
            if (string.IsNullOrEmpty(code))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("CODE_NOT_VALID");
                }
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                if (OnChangePasswordFailed != null)
                {
                    OnChangePasswordFailed.Invoke("NEW_PASSWORD_BLANK");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", code);
            if (!string.IsNullOrEmpty(phone))
            {
                data.Add("phone", phone);
            }
            if (!string.IsNullOrEmpty(email))
            {
                data.Add("email", email);
            }
            data.Add("new_password", Encryption.EncryptMD5(newPassword));
            Networking.SendRequest(CHANGE_PASSWORD, data);
        }

        public static void SendPasswordRecoveryCode(string email)
        {
            if (string.IsNullOrEmpty(email) || !Encryption.IsEmailValid(email))
            {
                if (OnPasswordRecoveryCodeSendFailed != null)
                {
                    OnPasswordRecoveryCodeSendFailed.Invoke("EMAIL_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("email", email);
            Networking.SendRequest(SEND_PASSWORD_RECOVERY_CODE, data);
        }

        public static void SendPasswordRecoveryCode(string phoneNumber, string phoneCountry)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(phoneCountry) || Encryption.IsPhoneNumberValid(phoneNumber, phoneCountry))
            {
                if (OnPasswordRecoveryCodeSendFailed != null)
                {
                    OnPasswordRecoveryCodeSendFailed.Invoke("PHONE_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("phone", phoneNumber);
            data.Add("country", phoneCountry);
            Networking.SendRequest(SEND_PASSWORD_RECOVERY_CODE, data);
        }

        public static void ChangeEmail(string email)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (!instance._authenticated)
            {
                if (OnChangeEmailFailed != null)
                {
                    OnChangeEmailFailed.Invoke("NOT_AUTHENTICATED");
                }
                return;
            }
            if (string.IsNullOrEmpty(email) || Encryption.IsEmailValid(email))
            {
                if (OnChangeEmailFailed != null)
                {
                    OnChangeEmailFailed.Invoke("EMAIL_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("email", email);
            data.Add("username", instance._username);
            data.Add("password", instance._password);
            data.Add("session", instance._session);
            Networking.SendRequest(CHANGE_EMAIL, data);
        }

        public static void ChangePhoneNumber(string number, string country)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (!instance._authenticated)
            {
                if (OnChangePhoneNumberFailed != null)
                {
                    OnChangePhoneNumberFailed.Invoke("NOT_AUTHENTICATED");
                }
                return;
            }
            if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(country)  || Encryption.IsPhoneNumberValid(number, country))
            {
                if (OnChangePhoneNumberFailed != null)
                {
                    OnChangePhoneNumberFailed.Invoke("PHONE_NUMBER_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("phone", number);
            data.Add("country", country);
            data.Add("username", instance._username);
            data.Add("password", instance._password);
            data.Add("session", instance._session);
            Networking.SendRequest(CHANGE_PHONE, data);
        }

        public static void VerifyEmail(string username, string code)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(code))
            {
                if (OnVerifyEmailFailed != null)
                {
                    OnVerifyEmailFailed.Invoke("CODE_NOT_VALID");
                }
                return;
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnVerifyEmailFailed != null)
                {
                    OnVerifyEmailFailed.Invoke("USER_NOT_EXIST");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", code);
            data.Add("username", username);
            Networking.SendRequest(VERIFY_EMAIL, data);
        }

        public static void VerifyEmail(int id, string code)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(code))
            {
                if (OnVerifyEmailFailed != null)
                {
                    OnVerifyEmailFailed.Invoke("CODE_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", code);
            data.Add("id", id);
            Networking.SendRequest(VERIFY_EMAIL, data);
        }

        public static void VerifyPhoneNumber(string username, string code)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(code))
            {
                if (OnVerifyPhoneNumberFailed != null)
                {
                    OnVerifyPhoneNumberFailed.Invoke("CODE_NOT_VALID");
                }
                return;
            }
            if (string.IsNullOrEmpty(username))
            {
                if (OnVerifyPhoneNumberFailed != null)
                {
                    OnVerifyPhoneNumberFailed.Invoke("USER_NOT_EXIST");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", code);
            data.Add("username", username);
            Networking.SendRequest(VERIFY_PHONE, data);
        }

        public static void VerifyPhoneNumber(int id, string code)
        {
            if (!instance || !instance.initialized)
            {
                Initialize();
            }
            if (string.IsNullOrEmpty(code))
            {
                if (OnVerifyPhoneNumberFailed != null)
                {
                    OnVerifyPhoneNumberFailed.Invoke("CODE_NOT_VALID");
                }
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", code);
            data.Add("id", id);
            Networking.SendRequest(VERIFY_PHONE, data);
        }

        private void Clear()
        {
            _username = "";
            _password = "";
            _session = "";
            _id = 0;
            _sessionId = 0;
        }

        private static string GetRandomString(int lenght)
        {
            string characters = "abcdefghijklmnopqrstuvwxyz1234567890-_!%^&()";
            string result = "";
            for (int i = 0; i < lenght; i++)
            {
                result += characters[UnityEngine.Random.Range(0, characters.Length)];
            }
            return result;
        }

        private static string GetRandomUsername()
        {
            string characters = "1234567890";
            string result = "Player";
            int length = instance.UsernameMaxLenght - result.Length;
            if (length <= 0)
            {
                characters = "abcdefghijklmnopqrstuvwxyz1234567890";
                result = "";
                length = instance.UsernameMaxLenght;
            }
            for (int i = 0; i < length; i++)
            {
                result += characters[UnityEngine.Random.Range(0, characters.Length)];
            }
            return result;
        }
        #endregion

        #region Control
        private void Hook(ref Dictionary<string, object> dataToSend, ref List<int> requestsToMake)
        {
            if (_authenticated)
            {
                requestsToMake.Add(UPDATE_USER_ACTIVITY);
                dataToSend.Add("username", _username);
                dataToSend.Add("password", _password);
                dataToSend.Add("session", _session);
            }
        }

        private void ProcessResponse(int requestID, JsonData data)
        {
            if(requestID == UPDATE_USER_ACTIVITY)
            {
                
            }
        }
        #endregion

    }
}
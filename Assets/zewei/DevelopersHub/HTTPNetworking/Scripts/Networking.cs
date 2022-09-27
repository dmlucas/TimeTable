namespace DevelopersHub.HTTPNetworking
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;
    using LitJson;
    using System.Linq;
    using System;

    public class Networking : MonoBehaviour
    {

        #region Events
        public static event ServerDataCallback OnRequestResponded;
        public static event NoCallback OnConnectedToServerSuccessful;

        public static event ErrorCallback OnConnectToServerFailed;
        public static event ErrorCallback OnDisconnectedFromServer;

        public static event DownloadCallback OnDownloadFileSuccessful;
        public static event DownloadFailedCallback OnDownloadFileFailed;
        #endregion

        [SerializeField] private Settings _settings = null;
        public Settings settings { get { return _settings; } set { _settings = value; } }
        private static Networking instance = null;
        public static Networking Instance { get { if (instance == null) { GetInstance(); } return instance; } }
        private static bool connected = false; public static bool IsConnectedToServer { get { return connected; } }
        private static bool syncConnection = true; public static bool SyncConnection { get { return syncConnection; } set { syncConnection = value; } }
        private int tryCount = 0;
        private float syncPeriod = 5;
        private float syncTimer = 0;
        private bool syncing = false;
        private static DateTime serverTime = DateTime.Now; public static DateTime ServerTime { get { return serverTime; } }
        private List<Extension> extensions = new List<Extension>(); public List<Extension> Extensions { get { return extensions; } }
        public delegate void ServerDataCallback(int requestID, bool successful, string error, JsonData data);
        public delegate void ErrorCallback(string error);
        public delegate void UsernameCallback(string username);
        public delegate void NoCallback();
        public delegate void JsonDataCallback(JsonData data);
        public delegate void DownloadCallback(string url, byte[] data);
        public delegate void DownloadFailedCallback(string url, string error);
        public delegate void RemainedTimeCallback(int remainedTime);

        private static int CONNECT_TO_SERVER = 987650;
        private static int SYNC_CONNECTION = 987651;

        public enum SortType { NONE = 0, ASCENDING = 1, DESCENDING = -1 }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private class RequestQuery
        {
            public RequestType type;
            public int id = 0;
            public Dictionary<string, object> data = null;
            public string url;
        }

        private List<RequestQuery> requests = new List<RequestQuery>();
        private static bool processing = false;

        private enum RequestType
        {
            normal, download
        }

        private void Update()
        {
            if (connected)
            {
                serverTime = serverTime.AddSeconds(Time.deltaTime);
                if (syncTimer >= syncPeriod && !syncing)
                {
                    syncTimer = 0;
                    Sync();
                }
                else
                {
                    syncTimer += Time.deltaTime;
                }
            }
            if (!processing && requests.Count > 0)
            {
                if (requests[0].type == RequestType.normal)
                {
                    processing = true;
                    Instance.StartCoroutine(Instance.Request(requests[0].id, requests[0].data));
                }
                else if (requests[0].type == RequestType.download)
                {
                    processing = true;
                    Instance.StartCoroutine(Instance.Download(requests[0].url));
                }
            }
        }

        private void Sync()
        {
            if (!syncConnection)
            {
                return;
            }
            syncing = true;
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<int> requests = new List<int>();
            for (int i = 0; i < extensions.Count; i++)
            {
                if (extensions[i].IsInitialized)
                {
                    Extension.HookData hook = extensions[i].Hook();
                    if(hook.requests != null && hook.requests.Length > 0)
                    {
                        requests.InsertRange(0, hook.requests);
                    }
                    if(hook.data != null && hook.data.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> item in hook.data)
                        {
                            if (data.ContainsKey(item.Key))
                            {
                                data[item.Key] = item.Value;
                            }
                            else
                            {
                                data.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
            }
            requests = requests.Distinct().ToList();
            if (requests.Count > 0)
            {
                data.Add("extentions_requests", requests.ToArray());
            }
            SendRequest(SYNC_CONNECTION, data);
        }

        private void SyncExtensions(bool successful, string error, JsonData data)
        {
            if (data != null)
            {
                for (int i = 0; i < extensions.Count; i++)
                {
                    extensions[i].Sync(data);
                }
            }
        }

        private void ProcessCoreResponse(int requestID, bool successful, string error, JsonData data)
        {

            #region Preliminary Checks
            if (successful)
            {
                successful = data.ContainsKey("successful") && bool.Parse(data["successful"].ToString());
                if (data.ContainsKey("error"))
                {
                    error = data["error"].ToString();
                }
            }
            else
            {
                if (connected)
                {
                    connected = false;
                    if (OnDisconnectedFromServer != null)
                    {
                        OnDisconnectedFromServer.Invoke(error);
                    }
                }
            }
            #endregion

            #region Secondary Checks
            bool notifyExtentions = false;
            if (successful)
            {
                if (requestID == CONNECT_TO_SERVER)
                {
                    connected = true;
                    if (data.ContainsKey("sync_period"))
                    {
                        syncPeriod = int.Parse(data["sync_period"].ToString());
                    }
                    if (OnConnectedToServerSuccessful != null)
                    {
                        OnConnectedToServerSuccessful.Invoke();
                    }
                }
                else if (requestID == SYNC_CONNECTION)
                {
                    if (data.ContainsKey("requests_response"))
                    {
                        data = data["requests_response"];
                    }
                    else
                    {
                        data = null;
                    }
                }
                else
                {
                    notifyExtentions = true;
                }
            }
            else
            {
                
                if (requestID == CONNECT_TO_SERVER)
                {
                    if (OnConnectToServerFailed != null)
                    {
                        OnConnectToServerFailed.Invoke(error);
                    }
                }
                else if (requestID == SYNC_CONNECTION)
                {
                    data = null;
                }
                else
                {
                    notifyExtentions = true;
                }
            }
            #endregion

            if (requestID == SYNC_CONNECTION)
            {
                SyncExtensions(successful, error, data);
            }

            if (notifyExtentions)
            {
                for (int i = 0; i < extensions.Count; i++)
                {
                    extensions[i].ProcessCoreRequest(requestID, successful, error, data);
                }
            }
        }

        private static void GetInstance()
        {
            Networking[] networks = FindObjectsOfType<Networking>();
            if (networks.Length == 0)
            {
                instance = new GameObject("HTTP_Networking").AddComponent<Networking>();
            }
            else
            {
                instance = networks[0];
                if (networks.Length > 1)
                {
                    for (int i = networks.Length - 2; i >= 0; i--)
                    {
                        DestroyImmediate(networks[i]);
                    }
                }
            }
            if(instance.settings == null)
            {
                try
                {
                    var resources = Resources.LoadAll("", typeof(Settings)).Cast<Settings>();
                    foreach (var settings in resources)
                    {
                        instance.settings = settings;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Try to connect the server.
        /// If successful then OnConnectToServerSuccessful event will be invoked.
        /// If failed then OnConnectToServerFailed event will be invoked and it will contain the error that caused the connection to fail.
        /// </summary>
        public static void ConnectToServer()
        {
            if (connected)
            {
                return;
            }
            SendRequest(CONNECT_TO_SERVER, null);
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        /// <param name="invokeEvent">If true then OnDisconnectedFromServer event will be invoked.</param>
        public static void DisconnectFromServer(bool invokeEvent = false)
        {
            if (!connected)
            {
                return;
            }
            connected = false;
            if (invokeEvent && OnDisconnectedFromServer != null)
            {
                OnDisconnectedFromServer.Invoke("USER_REQUEST");
            }
        }

        /// <summary>
        /// Sends a request to the server.
        /// </summary>
        /// <param name="requestID">Integer numer of your choosing between 0 and 987649 to identify the request.</param>
        /// <param name="data">Parameters for sending to the server. Pass null if you don't have any.</param>
        public static void SendRequest(int requestID, Dictionary<string, object> data)
        {
            RequestQuery request = new RequestQuery();
            request.id = requestID;
            request.data = data;
            request.type = RequestType.normal;
            Instance.requests.Add(request);
        }

        private IEnumerator Request(int requestID, Dictionary<string, object> data)
        {

            #region Create Essential Data
            string iv = Encryption.GenerateVI(16);
            string validation = Encryption.GenerateVI(UnityEngine.Random.Range(10, 30));
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            #endregion

            #region Write Json Data
            writer.WriteObjectStart();
            if(data != null)
            {
                foreach (KeyValuePair<string, object> item in data)
                {
                    if (item.Value is string)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((string)item.Value);
                    }
                    else if (item.Value is int)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((int)item.Value);
                    }
                    else if (item.Value is float)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((float)item.Value);
                    }
                    else if (item.Value is double)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((double)item.Value);
                    }
                    else if (item.Value is bool)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((bool)item.Value);
                    }
                    else if (item.Value is decimal)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((decimal)item.Value);
                    }
                    else if (item.Value is long)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((long)item.Value);
                    }
                    else if (item.Value is ulong)
                    {
                        writer.WritePropertyName(item.Key); writer.Write((ulong)item.Value);
                    }
                    else if (item.Value is int[])
                    {
                        writer.WritePropertyName(item.Key); writer.WriteArrayStart();
                        int[] array = (int[])item.Value;
                        for (int i = 0; i < array.Length; i++) { writer.Write(array[i]); }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is string[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        string[] array = (string[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is float[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        float[] array = (float[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is double[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        double[] array = (double[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is bool[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        bool[] array = (bool[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is decimal[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        decimal[] array = (decimal[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is long[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        long[] array = (long[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else if (item.Value is ulong[])
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteArrayStart();
                        ulong[] array = (ulong[])item.Value;
                        for (int i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i]);
                        }
                        writer.WriteArrayEnd();
                    }
                    else
                    {
                        writer.WritePropertyName(item.Key); writer.Write(item.Value.ToString());
                    }
                }
            }
            writer.WritePropertyName("request"); writer.Write(requestID);
            writer.WritePropertyName("validation"); writer.Write(validation);
            writer.WritePropertyName("version"); writer.Write(Application.version);
            writer.WriteObjectEnd();
            #endregion

            #region Initialize Form
            WWWForm form = new WWWForm();
            form.AddField("hash0", iv);
            form.AddField("hash1", Encryption.EncryptAES(sb.ToString(), iv, _settings.EncryptionKey));
            form.AddField("hash2", Encryption.EncryptMD5(validation, _settings.MD5Key));
            #endregion
          
            #region Web Request Method
            string result = "";
            //Debug.Log($"adress = {_settings.ApiAddress}");
            using (UnityWebRequest request = UnityWebRequest.Post(_settings.ApiAddress, form))
            {
                yield return request.SendWebRequest();
                bool networkError = false;
                #if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019
                networkError = request.isNetworkError || request.isHttpError;
                #else
                networkError = request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError;
                #endif
                if (networkError)
                {
                    Debug.LogError(request.error);
                    if (tryCount < 1)
                    {
                        tryCount++;
                        processing = false;
                        yield break;
                    }
                }
                else
                {
                    result = request.downloadHandler.text;
                }
                //Debug.Log(result);
            }
            #endregion

            #region WWW Method
            /*
            float connectionTimeout = 20f;
            WWW Data = new WWW(Settings.Instance.ApiAddress, form);
            float timer = 0;
            bool failed = false;
            while (!Data.isDone)
            {
                if (timer >= connectionTimeout) { failed = true; break; }
                timer += Time.deltaTime;
                yield return null;
            }
            if (failed || !string.IsNullOrEmpty(Data.error))
            {
                Debug.LogError(Data.error);
                Data.Dispose();
                OnRequestResponded?.Invoke(requestID, "", null);
                yield break;
            }
            yield return Data;
            string result = Data.text;
            */
            #endregion
  
            #region Process Response
            JsonData response = null;
            string error = "";
            bool successful = false;
            try
            {
                if (string.IsNullOrEmpty(result))
                {
                    error = "ERROR_CONNECTION";
                }
                else
                {
                    JsonData mainJason = JsonMapper.ToObject(result);
                    if (mainJason.ContainsKey("hash0") && mainJason.ContainsKey("hash1") && mainJason.ContainsKey("hash2"))
                    {
                        JsonData json = JsonMapper.ToObject(Encryption.DecryptAES(mainJason["hash0"].ToString(), mainJason["hash1"].ToString(), _settings.EncryptionKey));
                        if (json.ContainsKey("message") && Encryption.EncryptMD5(json["message"].ToString(), _settings.MD5Key) == mainJason["hash2"].ToString())
                        {
                            successful = (json["message"].ToString() == "SUCCESSFUL");
                            if (json.ContainsKey("error"))
                            {
                                error = json["error"].ToString();
                            }
                            if (json.ContainsKey("data"))
                            {
                                response = json["data"];
                                if (json["data"].ContainsKey("now_datetime"))
                                {
                                    DateTime.TryParse(json["data"]["now_datetime"].ToString(), out serverTime);
                                }
                            }
                        }
                        else
                        {
                            error = "ERROR_VALIDATION";
                        }
                    }
                    else
                    {
                        error = "ERROR_VALIDATION";
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                error = string.IsNullOrEmpty(result) ? "ERROR_CONNECTION" : "ERROR_JSON";
            }
            #endregion

            #region Invoke Events
            if (requestID >= 987650)
            {
                ProcessCoreResponse(requestID, successful, error, response);
                syncing = false;
            }
            else
            {
                OnRequestResponded?.Invoke(requestID, successful, error, response);
            }
            #endregion

            #region Final Actions
            processing = false;
            tryCount = 0;
            requests.RemoveAt(0);
            #endregion

        }

        /// <summary>
        /// Attempts to download a file.
        /// </summary>
        /// <param name="url">The link to the file.</param>
        public static void DownloadFile(string url)
        {
            RequestQuery request = new RequestQuery();
            request.url = url;
            request.type = RequestType.download;
            Instance.requests.Add(request);
        }

        private IEnumerator Download(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                bool networkError = false;
                #if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019
                networkError = request.isNetworkError || request.isHttpError;
                #else
                networkError = request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError;
                #endif
                if (networkError)
                {
                    Debug.LogError(request.error);
                    if (tryCount < 1)
                    {
                        tryCount++;
                        processing = false;
                        yield break;
                    }
                    if (OnDownloadFileFailed != null)
                    {
                        OnDownloadFileFailed.Invoke(url, request.error);
                    }
                }
                else
                {
                    if (OnDownloadFileSuccessful != null)
                    {
                        OnDownloadFileSuccessful.Invoke(url, request.downloadHandler.data);
                    }
                }
            }

            #region Final Actions
            processing = false;
            tryCount = 0;
            requests.RemoveAt(0);
            #endregion

        }

        public static void SaveFile(byte[] content, string path = "downloads/texts", string name = "report", string extention = "txt", bool overwite = true)
        {
            string savePath = string.Format("{0}/{1}/{3}.{4}", Application.persistentDataPath, path, name, extention);
            if (System.IO.File.Exists(savePath))
            {
                if (overwite)
                {
                    System.IO.File.Delete(savePath);
                }
                else
                {
                    return;
                }
            }
            System.IO.File.WriteAllBytes(savePath, content);
        }

        #region Deprecated
        /*
        This is not secure and makes server vulnerable to hackers.
        /// <summary>
        /// Sends a single MySQL query to the server.
        /// </summary>
        /// <param name="requestID">Integer numer of your choosing between 0 and 987649 to identify the request.</param>
        /// <param name="query">MySQL query.</param>
        public void SendSingleQueryRequest(int requestID, string query)
        {
            if (query.Count(x => (x == ';')) > 1)
            {
                // Query is not single
            }
            else
            {
                // Send the request
            }
        }
        */
        #endregion

    }
}
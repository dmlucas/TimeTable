namespace DevelopersHub.HTTPNetworking
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Collection of connection-relevant settings.
    /// </summary>
    public class Settings : ScriptableObject
    {

        [Header("API Credentials")]
        [Tooltip("Link to your php file on the server.")]
        [SerializeField] private string apiAddress = "https://demo.com/unity_projects/test/api.php";
        //[SerializeField] private string apiAddress = "https://baidu.com";
        public string ApiAddress { get { return apiAddress; } }

        [Tooltip("AES encryption password must be 32 characters. This password must be the same as the one in the server. This will be used to secure the data.")]
        [SerializeField] private string encryptionKey = "abcdefghijklmnopqrstuvwxyz012345";
        public string EncryptionKey { get { return encryptionKey; } }

        [Tooltip("MD5 encryption password can be any number of characters. This password must be the same as the one in the server.. This will be used to validate the data.")]
        [SerializeField] private string mD5Key = "abcdefg";
        public string MD5Key { get { return mD5Key; } }

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("Developers Hub/HTTP Networking/Settings")]
        public static void CreateSettings()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Settings).Name);
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                UnityEditor.EditorUtility.FocusProjectWindow();
                Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                UnityEditor.Selection.activeObject = obj;
            }
            else
            {
                string path = Application.dataPath + "/DevelopersHub/HTTPNetworking/Resources";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Settings asset = ScriptableObject.CreateInstance<Settings>();
                UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/DevelopersHub/HTTPNetworking/Resources/Settings.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.EditorUtility.FocusProjectWindow();
                UnityEditor.Selection.activeObject = asset;
            }
        }
        #endif

    }

    public class ReadOnlyAttribute : PropertyAttribute { }

    #if UNITY_EDITOR
    /// <summary>
    /// This class contain custom drawer for ReadOnly attribute.
    /// </summary>
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))] public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
    {
        /// <summary>
        /// Unity method for drawing GUI in Editor
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            // Saving previous GUI enabled value
            var previousGUIState = GUI.enabled;
            // Disabling edit for property
            GUI.enabled = false;
            // Drawing Property
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            // Setting old GUI enabled value
            GUI.enabled = previousGUIState;
        }
    }
    #endif

}
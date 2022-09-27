namespace DevelopersHub.HTTPNetworking
{
    using LitJson;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Extension : MonoBehaviour
    {

        protected bool initialized = false; public bool IsInitialized { get { return initialized; } }

        public class HookData
        {
            public Dictionary<string, object> data = new Dictionary<string, object>();
            public int[] requests = null;
        }

        public virtual HookData Hook()
        {
            return null;
        }

        public virtual void Sync(JsonData data)
        {
            
        }

        public virtual void ProcessCoreRequest(int requestID, bool successful, string error, JsonData data)
        {

        }

    }
}
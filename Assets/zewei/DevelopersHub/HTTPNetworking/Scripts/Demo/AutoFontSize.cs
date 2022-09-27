namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))] public class AutoFontSize : MonoBehaviour
    {

        [Range(0, 100)] public float size = 5f;
        private Text text = null;

        private void Start()
        {
            SetSize();
        }

        public void SetSize()
        {
            if (!text)
            {
                text = GetComponent<Text>();
            }
            text.fontSize = Mathf.RoundToInt(Screen.height * size / 100f);
        }

        private void OnEnable()
        {
            SetSize();
        }

    }
}
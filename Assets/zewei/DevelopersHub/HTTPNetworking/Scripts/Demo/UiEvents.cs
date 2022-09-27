namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public class UiEvents : MonoBehaviour
    {

        [Space]
        public UnityEvent OnRectTransformSizeChange = new UnityEvent();

        private void OnRectTransformDimensionsChange()
        {
            OnRectTransformSizeChange.Invoke();
        }

        private static UiEvents instance = null;

        public static UiEvents Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<UiEvents>();
                    if (!instance)
                    {
                        Canvas canvas = FindObjectOfType<Canvas>();
                        if (!canvas)
                        {
                            canvas = new GameObject("Canvas").AddComponent<Canvas>();
                        }
                        instance = canvas.gameObject.AddComponent<UiEvents>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

    }
}
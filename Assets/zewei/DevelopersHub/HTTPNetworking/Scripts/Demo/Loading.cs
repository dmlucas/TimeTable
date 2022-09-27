namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class Loading : MonoBehaviour
    {

        private bool animate = false;
        [SerializeField] [Range(0f, 0.1f)] private float delay = 0.01f;
        [SerializeField] private Sprite[] sprites = null;
        private float timer = 0;
        private int index = 0;
        [SerializeField] private Image image = null;
        [SerializeField] private Image background = null;

        private static Loading instance = null;
        public static Loading Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<Loading>();
                    if (!instance)
                    {
                        Canvas canvas = FindObjectOfType<Canvas>();
                        if (!canvas)
                        {
                            canvas = new GameObject("Canvas").AddComponent<Canvas>();
                        }
                        var resources = Resources.LoadAll("", typeof(Loading)).Cast<Loading>();
                        foreach (var loading in resources)
                        {
                            instance = Instantiate(loading, canvas.transform);
                            break;
                        }
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (!animate)
            {
                return;
            }
            if(timer >= delay)
            {
                timer = 0;
                index++;
                if(index >= sprites.Length)
                {
                    index = 0;
                }
                image.sprite = sprites[index];
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        public static void Show()
        {
            Instance.animate = true;
            Instance.background.gameObject.SetActive(true);
            Transform parent = Instance.transform.parent;
            if (parent)
            {
                Instance.transform.SetSiblingIndex(parent.childCount - 1);
            }
        }

        public static void Hide()
        {
            Instance.animate = false;
            Instance.background.gameObject.SetActive(false);
        }

    }
}
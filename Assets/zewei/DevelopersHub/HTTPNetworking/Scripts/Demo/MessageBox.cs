namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using System.Linq;

    public class MessageBox : MonoBehaviour
    {

        public enum Type
        {
            success, error, warning, info, question
        }

        [Header("Events")]
        public UnityEvent onButton1Clicked = new UnityEvent();
        public UnityEvent onButton2Clicked = new UnityEvent();

        [Header("Main")]
        [SerializeField] private Image background = null;

        [Header("Portrait")]
        [SerializeField] private Image backgroundPortrait = null;
        [SerializeField] private Image iconPortrait = null;
        [SerializeField] private Text titlePortrait = null;
        [SerializeField] private Text textPortrait = null;
        [SerializeField] private Text button1TextPortrait = null;
        [SerializeField] private Text button2TextPortrait = null;
        [SerializeField] private Text button3TextPortrait = null;
        [SerializeField] private Button button1Portrait = null;
        [SerializeField] private Button button2Portrait = null;
        [SerializeField] private Button button3Portrait = null;

        [Header("Landscape")]
        [SerializeField] private Image backgroundLandscape = null;
        [SerializeField] private Image iconLandscape = null;
        [SerializeField] private Text titleLandscape = null;
        [SerializeField] private Text textLandscape = null;
        [SerializeField] private Text button1TextLandscape = null;
        [SerializeField] private Text button2TextLandscape = null;
        [SerializeField] private Text button3TextLandscape = null;
        [SerializeField] private Button button1Landscape = null;
        [SerializeField] private Button button2Landscape = null;
        [SerializeField] private Button button3Landscape = null;

        [Header("Color")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private Color warningColor = Color.magenta;
        [SerializeField] private Color infoColor = Color.blue;
        [SerializeField] private Color questionColor = Color.cyan;

        [Header("Icon")]
        [SerializeField] private Sprite successIcon = null;
        [SerializeField] private Sprite errorIcon = null;
        [SerializeField] private Sprite warningIcon = null;
        [SerializeField] private Sprite infoIcon = null;
        [SerializeField] private Sprite questionIcon = null;

        private static MessageBox instance = null;
        public static MessageBox Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<MessageBox>();
                    if (!instance)
                    {
                        Canvas canvas = FindObjectOfType<Canvas>();
                        if (!canvas)
                        {
                            canvas = new GameObject("Canvas").AddComponent<Canvas>();
                        }
                        var resources = Resources.LoadAll("", typeof(MessageBox)).Cast<MessageBox>();
                        foreach (var msg in resources)
                        {
                            instance = Instantiate(msg, canvas.transform);
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

        private void Start()
        {
            UiEvents.Instance.OnRectTransformSizeChange.AddListener(OnScreenSizeChanged);
            button1Landscape.onClick.AddListener(Button1Click);
            button2Landscape.onClick.AddListener(Button1Click);
            button3Landscape.onClick.AddListener(Button2Click);
            button1Portrait.onClick.AddListener(Button1Click);
            button2Portrait.onClick.AddListener(Button1Click);
            button3Portrait.onClick.AddListener(Button2Click);
        }

        private void OnScreenSizeChanged()
        {
            bool portrait = (Screen.width < Screen.height);
            instance.backgroundPortrait.gameObject.SetActive(portrait);
            instance.backgroundLandscape.gameObject.SetActive(!portrait);
        }

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }

        private void Button1Click()
        {
            if (onButton1Clicked != null)
            {
                onButton1Clicked.Invoke();
            }
            Close();
        }

        private void Button2Click()
        {
            if (onButton2Clicked != null)
            {
                onButton2Clicked.Invoke();
            }
            Close();
        }

        private void Close()
        {
            Instance.background.gameObject.SetActive(false);
            Loading.Hide();
        }

        public static void ShowMessage(string title, string text, Type type, string button)
        {
            if (Instance == null)
            {
                return;
            }
            Color color = Color.white;
            instance.ShowMessage(title, text, button, button, button, instance.GetColor(type), instance.GetIcon(type));
            Loading.Hide();
        }

        private Color GetColor(Type type)
        {
            switch (type)
            {
                case Type.success:
                    return successColor;
                case Type.error:
                    return errorColor;
                case Type.warning:
                    return warningColor;
                case Type.info:
                    return infoColor;
                case Type.question:
                    return questionColor;
            }
            return Color.white;
        }

        private Sprite GetIcon(Type type)
        {
            switch (type)
            {
                case Type.success:
                    return successIcon;
                case Type.error:
                    return errorIcon;
                case Type.warning:
                    return warningIcon;
                case Type.info:
                    return infoIcon;
                case Type.question:
                    return questionIcon;
            }
            return null;
        }

        private void ShowMessage(string title, string text, string button1, string button2, string button3, Color color, Sprite icon)
        {
            titleLandscape.text = title;
            titlePortrait.text = title;
            textPortrait.text = text;
            textLandscape.text = text;
            button1TextPortrait.text = button1;
            button1TextLandscape.text = button1;
            button2TextPortrait.text = button2;
            button2TextLandscape.text = button2;
            button3TextPortrait.text = button3;
            button3TextLandscape.text = button3;
            backgroundLandscape.color = color;
            backgroundPortrait.color = color;
            iconLandscape.sprite = icon;
            iconPortrait.sprite = icon;
            bool portrait = (Screen.width < Screen.height);
            instance.background.gameObject.SetActive(true);
            instance.backgroundPortrait.gameObject.SetActive(portrait);
            instance.backgroundLandscape.gameObject.SetActive(!portrait);
            Transform parent = transform.parent;
            if (parent)
            {
                transform.SetSiblingIndex(parent.childCount - 1);
            }
        }

    }
}
namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BatchColorChange : MonoBehaviour
    {

        [System.Serializable] public class Element
        {
            public string name = "";
            public Color color = Color.white;
            public Image[] images = null;
            public Text[] texts = null;
        }

        [SerializeField] private Element[] elements = null;

        private void Awake()
        {
            if (elements != null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].images != null)
                    {
                        for (int j = 0; j < elements[i].images.Length; j++)
                        {
                            elements[i].images[j].color = elements[i].color;
                        }
                    }
                    if (elements[i].texts != null)
                    {
                        for (int j = 0; j < elements[i].texts.Length; j++)
                        {
                            elements[i].texts[j].color = elements[i].color;
                        }
                    }
                }
            }
            Destroy(this);
        }

    }
}
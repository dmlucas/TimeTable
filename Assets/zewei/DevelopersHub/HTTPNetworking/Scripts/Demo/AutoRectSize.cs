namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AutoRectSize : MonoBehaviour
    {

        public enum Type
        {
            makeWidthEqualsInput, makeHeightEqualsInput, makeWidthEqualsHeight, makeHeightEqualsWidth
        }

        [SerializeField] private float input = 5;
        [SerializeField] private Type type = Type.makeWidthEqualsHeight;
        [SerializeField] private int delay = 1;
        private RectTransform rect = null;

        private IEnumerator Start()
        {
            for (int i = 0; i < delay; i++)
            {
                yield return null;
            }
            rect = GetComponent<RectTransform>();
            if (rect)
            {
                float size = 0;
                switch (type)
                {
                    case Type.makeWidthEqualsHeight:
                        size = rect.sizeDelta.y;
                        rect.sizeDelta = new Vector2(size, size);
                        break;
                    case Type.makeHeightEqualsWidth:
                        size = rect.sizeDelta.x;
                        rect.sizeDelta = new Vector2(size, size);
                        break;
                    case Type.makeWidthEqualsInput:
                        size = Screen.height * input / 100f;
                        rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);
                        break;
                    case Type.makeHeightEqualsInput:
                        size = Screen.height * input / 100f;
                        rect.sizeDelta = new Vector2(rect.sizeDelta.x, size);
                        break;
                }
            }
            Destroy(this);
        }

    }
}
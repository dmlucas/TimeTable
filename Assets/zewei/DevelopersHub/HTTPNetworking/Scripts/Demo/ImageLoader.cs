namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ImageLoader : MonoBehaviour
    {

        private Image image = null;
        private string imageUrl = "";
        private bool loading = false;

        public void LoadImage(string url)
        {
            image = GetComponent<Image>();
            if (image && !string.IsNullOrEmpty(url))
            {
                imageUrl = url;
                Networking.OnDownloadFileSuccessful += Networking_OnDownloadFileSuccessful;
                Networking.OnDownloadFileFailed += Networking_OnDownloadFileFailed;
                Networking.DownloadFile(url);
                loading = true;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (loading)
            {
                Networking.OnDownloadFileSuccessful -= Networking_OnDownloadFileSuccessful;
                Networking.OnDownloadFileFailed -= Networking_OnDownloadFileFailed;
            }
        }

        private void Networking_OnDownloadFileFailed(string url, string error)
        {
            if(url == imageUrl)
            {
                loading = false;
                Networking.OnDownloadFileSuccessful -= Networking_OnDownloadFileSuccessful;
                Networking.OnDownloadFileFailed -= Networking_OnDownloadFileFailed;
                Destroy(this);
            }
        }

        private void Networking_OnDownloadFileSuccessful(string url, byte[] data)
        {
            if (url == imageUrl)
            {
                Networking.OnDownloadFileSuccessful -= Networking_OnDownloadFileSuccessful;
                Networking.OnDownloadFileFailed -= Networking_OnDownloadFileFailed;
                Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                text.LoadImage(data);
                image.sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(text.width / 2f, text.height / 2f));
                Destroy(this);
            }
        }

    }
}
namespace DevelopersHub.HTTPNetworking.Tools
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UserListItem : MonoBehaviour
    {

        [SerializeField] private Image _picture = null;
        [SerializeField] private GameObject _verified = null;
        [SerializeField] private Text _username = null;
        [SerializeField] private Text _rank = null;
        [SerializeField] private Text _score = null;

        public void Initialize(string username, string pictureUrl, int rank, int score, bool verified)
        {
            _username.text = username;
            _rank.text = rank.ToString();
            _score.text = score.ToString();
            _verified.SetActive(verified);
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                ImageLoader loader = _picture.gameObject.AddComponent<ImageLoader>();
                loader.LoadImage(pictureUrl);
            }
        }

    }
}
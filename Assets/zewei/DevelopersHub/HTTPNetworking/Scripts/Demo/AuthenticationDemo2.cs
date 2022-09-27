using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopersHub.HTTPNetworking;
using DevelopersHub.HTTPNetworking.Tools;
using UnityEngine.UI;

public class AuthenticationDemo2 : MonoBehaviour
{

    [SerializeField] public GameObject background = null;
    [SerializeField] private GameObject profilePage = null;
    [SerializeField] private GameObject usersListPage = null;
    [SerializeField] private GameObject portraitProfile = null;
    [SerializeField] private GameObject landscapeProfile = null;
    [SerializeField] private GameObject portraitUsers = null;
    [SerializeField] private GameObject landscapeUsers = null;
    [SerializeField] private RectTransform userListRect = null;

    [SerializeField] private Text usernameText = null;
    [SerializeField] private Text scoreText = null;
    [SerializeField] private GameObject verifiedIcion = null;
    [SerializeField] private Button rankingButtonPortrait = null;
    [SerializeField] private Button rankingButtonLandscape = null;

    [SerializeField] private Button nextUsersPage = null;
    [SerializeField] private Button prevUsersPage = null;
    [SerializeField] private Text pageText = null;

    [SerializeField] private UserListItem userItem = null;
    [SerializeField] private Transform usersParent = null;

    [SerializeField] private Button rankingBackPortrait = null;
    [SerializeField] private Button rankingBackLandscape = null;

    private int usersPage = 1;
    private Page page = Page.profile;
    private List<UserListItem> userItems = new List<UserListItem>();

    private enum Page
    {
        profile, users
    }

    private void Start()
    {
        UiEvents.Instance.OnRectTransformSizeChange.AddListener(OnScreenSizeChanged);
        OnScreenSizeChanged();
        rankingButtonPortrait.onClick.AddListener(OpenUsersList);
        rankingButtonLandscape.onClick.AddListener(OpenUsersList);
        rankingBackLandscape.onClick.AddListener(Back);
        rankingBackPortrait.onClick.AddListener(Back);
        nextUsersPage.onClick.AddListener(NextUsers);
        prevUsersPage.onClick.AddListener(PrevUsers);
    }

    public void Initialize(string username, bool verified, int score)
    {
        usernameText.text = username;
        scoreText.text = "Score: " + score.ToString();
        verifiedIcion.SetActive(verified);
        background.SetActive(true);
        OpenPage(Page.profile);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    private void Back()
    {
        if(page == Page.users)
        {
            OpenPage(Page.profile);
            ClearUserItems();
        }
    }

    private void OpenUsersList()
    {
        Loading.Show();
        Authentication.OnGetUsersDataPerPageFailed += Authentication_OnGetUsersDataPerPageFailed;
        Authentication.OnGetUsersDataPerPageSuccessful += Authentication_OnGetUsersDataPerPageSuccessful;
        Authentication.GetUsersDataPerPage(1, 10, Networking.SortType.DESCENDING, "score");
    }

    private void NextUsers()
    {
        Loading.Show();
        Authentication.GetUsersDataPerPage(usersPage + 1, 10, Networking.SortType.DESCENDING, "score");
    }

    private void PrevUsers()
    {
        Loading.Show();
        Authentication.GetUsersDataPerPage(usersPage - 1, 10, Networking.SortType.DESCENDING, "score");
    }

    private void Authentication_OnGetUsersDataPerPageSuccessful(LitJson.JsonData users, int totalUsersCount, int currentPage, int countPerPage)
    {
        pageText.text = currentPage.ToString();
        ClearUserItems();
        for (int i = 0; i < users.Count; i++)
        {
            UserListItem item = Instantiate(userItem, usersParent);
            string username = users[i]["username"].ToString();
            int rank = int.Parse(users[i]["rank"].ToString());
            int score = int.Parse(users[i]["score"].ToString());
            bool verified = (int.Parse(users[i]["is_verified"].ToString()) > 0);
            string url = users[i]["picture_url"].ToString();
            item.Initialize(username, url, rank, score, verified);
            userItems.Add(item);
        }
        nextUsersPage.interactable = (totalUsersCount > currentPage * countPerPage);
        prevUsersPage.interactable = (currentPage > 1);
        usersPage = currentPage;
        OpenPage(Page.users);
        Loading.Hide();
    }

    private void ClearUserItems()
    {
        for (int i = 0; i < userItems.Count; i++)
        {
            Destroy(userItems[i].gameObject);
        }
        userItems.Clear();
    }

    private void Authentication_OnGetUsersDataPerPageFailed(string error)
    {
        MessageBox.ShowMessage("ERROR!", "Failed to get the users list.", MessageBox.Type.error, "OK");
    }

    private void OpenPage(Page p)
    {
        profilePage.SetActive(p == Page.profile);
        usersListPage.SetActive(p == Page.users);
        page = p;
    }

    private void OnScreenSizeChanged()
    {
        bool portrait = (Screen.width < Screen.height);
        if (portrait)
        {
            userListRect.anchorMin = new Vector2(0.043f, 0);
            userListRect.anchorMax = new Vector2(0.959f, 0.892f);
        }
        else
        {
            userListRect.anchorMin = new Vector2(0.35f, 0);
            userListRect.anchorMax = new Vector2(0.65f, 0.892f);
        }
        userListRect.anchoredPosition = Vector3.zero;
        portraitUsers.SetActive(portrait);
        landscapeUsers.SetActive(!portrait);
        portraitProfile.SetActive(portrait);
        landscapeProfile.SetActive(!portrait);
    }

}
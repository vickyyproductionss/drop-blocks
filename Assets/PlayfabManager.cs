using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;

    string PlayFabId;


    [Header("Menu scene requirements")]
    public GameObject itemprefab;
    public GameObject leaderboardContentParent;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Login();
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            if(!PlayerPrefs.HasKey("Username"))
            {
                MenuManager.instance.NameInputScreen.SetActive(true);
            }
        }
    }


    #region Login
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true // Create account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged in to PlayFab!");
        PlayFabId = result.PlayFabId;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        PlayFabId = "null";
        Debug.LogError("Failed to log in to PlayFab: " + error.ErrorMessage);
    }
    #endregion

    #region Get Top leaderboard
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0, // Start from the top
            MaxResultsCount = 100, // Get the top 100 players
            StatisticName = "Highscore"
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        MenuManager.instance.homescreen.SetActive(false);
        MenuManager.instance.leaderboardscreen.SetActive(true);
        if(leaderboardContentParent.transform.childCount != 0)
        {
            for (int i = leaderboardContentParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leaderboardContentParent.transform.GetChild(i).gameObject);
            }
        }
        Debug.Log("Got leaderboard data!");
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + ": " + item.DisplayName + " - " + item.StatValue);
            GameObject itemgo = Instantiate(itemprefab, transform.position, Quaternion.identity);
            itemgo.transform.SetParent(leaderboardContentParent.transform, false);
            itemgo.transform.GetChild(0).GetComponent<TMP_Text>().text = (item.Position + 1).ToString()+".";
            itemgo.transform.GetChild(2).GetComponent<TMP_Text>().text = (item.StatValue + 1).ToString();
            if(item.DisplayName != null)
            {
                itemgo.transform.GetChild(1).GetComponent<TMP_Text>().text = (item.DisplayName + 1).ToString();
            }
            else
            {
                itemgo.transform.GetChild(1).GetComponent<TMP_Text>().text = "Player7861";
            }
        }
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get leaderboard data: " + error.ErrorMessage);
    }
    #endregion

    #region Send scores

    public void SubmitScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new System.Collections.Generic.List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Highscore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSubmitScoreSuccess, OnSubmitScoreFailure);
    }

    private void OnSubmitScoreSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score submitted to leaderboard!");
    }

    private void OnSubmitScoreFailure(PlayFabError error)
    {
        Debug.LogError("Failed to submit score to leaderboard: " + error.ErrorMessage);
    }

    #endregion

    #region Display name

    public void UpdateDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully!");
        PlayerPrefs.SetString("Username", result.DisplayName);
        MenuManager.instance.NameInputScreen.SetActive(false);
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.ErrorMessage);
    }

    #endregion

}

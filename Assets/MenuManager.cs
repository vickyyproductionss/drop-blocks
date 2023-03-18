using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject homescreen;
    public GameObject leaderboardscreen;
    public GameObject NameInputScreen;

    public static MenuManager instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        homescreen.SetActive(true);
        leaderboardscreen.SetActive(false);
    }

    public void OnClickUpdateName(TMP_InputField input)
    {
        if(input != null)
        {
            PlayfabManager.Instance.UpdateDisplayName(input.text);
        }
        else
        {
            PlayfabManager.Instance.UpdateDisplayName("Vickyy");
        }
    }

    public void OnClickGetleaderboard()
    {
        PlayfabManager.Instance.GetLeaderboard();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class endGameScript : MonoBehaviour
{
    public TMP_Text Text;
    void Start()
    {
        Text.text = "Score: " + GameManager2.instance.fromIntToString(GameManager2.instance.score) +"\n" + "Highscore: " + GameManager2.instance.fromIntToString(PlayerPrefs.GetInt("Highscore"));
    }
}

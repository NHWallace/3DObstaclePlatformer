using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Written with the intended mechanic of always giving the 1st star no matter what

    [Header("Components")]
    [SerializeField] GameObject levelCompleteScreen;
    [SerializeField] GameObject timerObject;
    [SerializeField] TMP_Text cashLabel;
    [SerializeField] Image star2;
    [SerializeField] Image star3;
    [SerializeField] Color starObtainedColor;


    [Header("Level settings")]
    [SerializeField] int baseCurrency;
    [SerializeField] float timeFor3Stars;
    [SerializeField] float timeFor2Stars;


    private Timer timer;
    private float timeTaken;

    public void Awake() {
        timer = timerObject.GetComponent<Timer>();
    }

    public void TriggerLevelCompletion() {
        timeTaken = timer.GetTime();
        Time.timeScale = 0;
        int starsObtained = 0;

        if (timeTaken < timeFor3Stars) {
            // Player completed level with 3 stars
            starsObtained = 3;
            star3.color = starObtainedColor;
            star2.color = starObtainedColor;
        }
        else if (timeTaken < timeFor2Stars) {
            // Player completed level with 2 stars
            starsObtained = 2;
            star2.color = starObtainedColor;
        }
        else {
            // Player completed level with 1 star
            starsObtained = 1;
        }

        float currencyObtained = baseCurrency * starsObtained;

        cashLabel.text = "+" + currencyObtained;

        levelCompleteScreen.SetActive(true);
    }

    public void GoToLevel(string levelName) {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    public void GoToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

}
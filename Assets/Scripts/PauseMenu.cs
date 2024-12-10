using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pausemenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Slider musicSlider;

    public void Update() {
        if (pauseMenu.activeSelf == false && Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
        else if (pauseMenu.activeSelf == true && Input.GetKeyDown(KeyCode.Escape)) {
            Unpause();
        }
    }

    public void Pause() {
        pauseMenu.SetActive(true);
        //Time.timeScale = 0;
    }

    public void Unpause() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void SetEffectsvolume() {
        // Workaround as OnValueChanged for sliders is bugged
        AudioManager.Instance.SetEffectsVolume(effectsSlider.value);
    }

    public void SetMusicVolume() {
        // Workaround as OnValueChanged for sliders is bugged
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Singleton design pattern class
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, effects;
    public AudioSource musicSource, effectsSource, runningSource; // Workaround for walking not being a clip for a single step

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void Start() {
        PlayMusic("BGM");
    }

    public void PlayMusic(string musicName) {
        Sound musicToPlay = Array.Find(music, x => x.name == musicName);

        if (musicToPlay == null) {
            Debug.Log("Music not found, check name. Case sensitive.");
        }
        else {
            musicSource.clip = musicToPlay.clip;
            musicSource.Play();
        }
    }

    public void PlayEffect(string effectName) {
        Sound effectToPlay = Array.Find(effects, x => x.name == effectName);

        if (effectToPlay == null) {
            Debug.Log("Effect not found, check name. Case sensitive.");
        }
        else {
            effectsSource.PlayOneShot(effectToPlay.clip);
        }
    }

    public void PlayRunningSound(string effectName) {
        Sound effectToPlay = Array.Find(effects, x => x.name == effectName);

        if (effectToPlay == null) {
            Debug.Log("Effect not found, check name. Case sensitive.");
        }
        else {
            runningSource.clip = (effectToPlay.clip);

            if (!runningSource.isPlaying) {
                runningSource.Play();
            }
        }
    }

    public void PauseRunningSound() {
        runningSource.Pause();
    }
}

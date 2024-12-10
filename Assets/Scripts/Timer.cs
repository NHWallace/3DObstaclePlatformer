using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text timerLabel;

    private float timer = 0f;
    private int minutes;
    private float seconds;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        seconds = timer % 60;
        minutes = (int)timer / 60;

        timerLabel.text = "Time taken: " + minutes + ":" + string.Format("{0:00.000}", seconds);
    }

    public float GetTime() {
        return this.timer;
    }
}

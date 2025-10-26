using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TimerScript : MonoBehaviour
{
    public float timeRemaining;
    bool timerIsRunning;
    public TMP_Text text;
    public UnityEvent gameOver;

    private void Start()
    {
        timerIsRunning = true;
    }
    void Update()
    {
        text.text = Mathf.Round(timeRemaining).ToString();
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                Time.timeScale = 0;
                gameOver.Invoke();
            }
        }
    }
}

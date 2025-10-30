using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    public float elapsedTime, remainingTime;

    void Start()
    {
        remainingTime = GameManager.RecordTime;
    }
    void Update()
    {
        if (GameManager.Mode == GameManager.PlayMode.Recording)
        {
            Timer();
        }
        else
        {
            Countdown();
        }
    }

    private void Timer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Countdown()
    {
        remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (remainingTime <= 0f)
        {
            GameObject.Find("UIManager").GetComponent<InGameUIManager>().LooseUI();
        }
    }
}

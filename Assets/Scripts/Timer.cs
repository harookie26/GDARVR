using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    bool startTime = false;
    float currentTime = 0f;
    [SerializeField] private float maxTime = 300f;

    bool isTimerRunning;
    bool gameOver = false;

    private void Start()
    {
        startTime = true;
        isTimerRunning = true;

        currentTime = maxTime;
    }

    private void Update()
    {
        if (startTime)
        {
            isTimerRunning = true;
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                OnTimerDone();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTime()
    {
        startTime = false;
        isTimerRunning = false;
    }

    public void OnTimerDone()
    {
        currentTime = 0f;
        startTime = false;
        gameOver = true;

        isTimerRunning = false;
    }

    public bool GetTimerRunning()
    {
        return isTimerRunning;
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

}

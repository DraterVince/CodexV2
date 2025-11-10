using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
<<<<<<< HEAD
    [SerializeField] public float remainingTime;
=======
    [SerializeField] float remainingTime;
>>>>>>> 67f9692 (Initial Commit: Working Gameplay, Working Level Select, Need Polish.)
    [SerializeField] GameObject GameOverScreen;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            timerText.color = Color.red;
            GameOverScreen.SetActive(true);
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
<<<<<<< HEAD
    public void ResetTimer()
    {
        float resetTime = 20;
        timerText.text = resetTime.ToString();
    }
=======
>>>>>>> 67f9692 (Initial Commit: Working Gameplay, Working Level Select, Need Polish.)
}
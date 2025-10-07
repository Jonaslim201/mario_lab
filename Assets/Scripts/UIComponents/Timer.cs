using UnityEngine;
using TMPro;

//Remeber to reset timer on reset

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float remainingTime; //in seconds

    private bool isActive;
    private float initalTime;
    public PlayerMovement PlayerMovement;
    // Update is called once per frame
    private void Start()
    {
        isActive = true;
        initalTime = remainingTime;

    }
    void Update()
    {
        if (isActive)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else if (remainingTime < 0)
            {
                remainingTime = 0;
                isActive = false;
                //Insert function when timer hits 0
                PlayerMovement.OnDeath();
            }
            int min = Mathf.FloorToInt(remainingTime / 60);
            int sec = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", min, sec);

        }
    }

    void OnEnable()
    {
        GameManager.OnGameRestart += HandleGameRestart;
    }

    void OnDisable()
    {
        GameManager.OnGameRestart -= HandleGameRestart;
    }

    private void HandleGameRestart()
    {
        SetRestart();
    }

    public void SetRestart()
    {
        remainingTime = initalTime;
        isActive = true;
    }
}

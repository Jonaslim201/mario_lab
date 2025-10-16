using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/ScoreData")]
public class ScoreData : ScriptableObject
{
    public int currentScore = 0;
    public int highScore = 0;

    public void ResetCurrentScore()
    {
        currentScore = 0;
        PlayerPrefs.SetInt("CurrentScore", currentScore);
    }

    public void ResetHighScore()
    {
        Debug.Log("High score reset to 0");
        highScore = 0;
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        if (currentScore > highScore)
        {
            highScore = currentScore;
        }
    }


}

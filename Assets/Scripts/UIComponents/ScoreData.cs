using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/ScoreData")]
public class ScoreData : ScriptableObject
{
    public int currentScore = 0;
    public int highScore = 0;

    public void ResetCurrentScore()
    {
        currentScore = 0;
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

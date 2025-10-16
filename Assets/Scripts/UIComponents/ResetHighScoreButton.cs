using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ResetHighScoreButton : MonoBehaviour, InteractiveButton
{
    [SerializeField] private ScoreData scoreData;
    public static event Action<int> OnHighScoreReset;
    public void ButtonClick()
    {
        Debug.Log("Reset High Score Button Clicked");
        if (scoreData != null)
        {
            scoreData.ResetHighScore();
            OnHighScoreReset?.Invoke(0);
        }
        else
        {
            Debug.LogWarning("ScoreData reference is not set in ResetHighScoreButton!");
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RestartButton : MonoBehaviour, InteractiveButton
{
    [SerializeField] private GameManager gameManager;
    public void ButtonClick()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            Debug.LogWarning("GameManager reference is not set in RestartButton!");
        }
    }
}

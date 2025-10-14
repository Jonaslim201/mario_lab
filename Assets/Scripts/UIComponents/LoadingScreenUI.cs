using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreenUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    [SerializeField] private ScoreData scoreData;
    private Coroutine loadCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadCoroutine = StartCoroutine(LoadNextSceneAfterDelay());
        scoreText.text = "Highscore " + scoreData.highScore.ToString();
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
    }


    public void BackToMainMenu()
    {
        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
        SceneManager.LoadScene("MainMenu");
    }

}

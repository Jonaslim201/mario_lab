using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoManager : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Video Library")]
    [SerializeField] private VideoClip[] videoClips;

    [Header("Raw Image Display")]
    [SerializeField] public RawImage killImage;

    private RenderTexture rt;

    void Start()
    {
        killImage.enabled = true;
        videoPlayer.clip = null;
        rt = videoPlayer.targetTexture;
    }

    public void PlayVideo(int index)
    {
        PlayVideoByIndex(index);
    }

    void OnEnable()
    {
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameRestart += HandleGameRestart;
    }

    void OnDisable()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameRestart -= HandleGameRestart;
    }

    private void PlayVideoByIndex(int index)
    {
        Debug.Log($"Playing video at index {index}");
        StopVideo();
        if (videoPlayer != null && videoClips != null &&
            index >= 0 && index < videoClips.Length &&
            videoClips[index] != null)
        {
            videoPlayer.clip = videoClips[index];
            ClearVideoDisplay();
            MuteAllAudioTracks();
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning($"Cannot play video at index {index}. Check VideoPlayer and clips array.");
        }
    }

    public void HandleGameStart()
    {
        TurnOffVideoDisplay();
    }

    public void HandleGameOver(int score)
    {
        TurnOffVideoDisplay();
    }

    public void HandleGameRestart()
    {
        TurnOffVideoDisplay();
    }

    public void TurnOffVideoDisplay()
    {
        ClearVideoDisplay();
        StopVideo();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;
    }

    private void MuteAllAudioTracks()
    {
        for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
        {
            videoPlayer.SetDirectAudioMute(i, true);
        }
    }

    private void ClearVideoDisplay()
    {
        if (rt != null)
        {
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }
    }
}
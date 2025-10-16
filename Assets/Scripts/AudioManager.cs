using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Library")]
    [SerializeField] private AudioClip[] soundClips;

    void Start()
    {

    }

    public void PlaySound(int index)
    {
        PlaySoundByIndex(index);
    }

    public void PlayOnLoop(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Cannot play sound on loop. Check AudioSource and clip.");
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Cannot play sound. Check AudioSource and clip.");
        }
    }

    private void PlaySoundByIndex(int index)
    {
        Debug.Log($"Playing sound at index {index}");
        StopSound();
        if (audioSource != null && soundClips != null &&
            index >= 0 && index < soundClips.Length &&
            soundClips[index] != null)
        {
            audioSource.PlayOneShot(soundClips[index]);
        }
        else
        {
            Debug.LogWarning($"Cannot play sound at index {index}. Check AudioSource and clips array.");
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}
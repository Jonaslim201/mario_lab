using System.Collections;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] public AudioClip coinPickUpSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "AudioSource component missing from CoinBehavior");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCoinSound()
    {
        Debug.Log("Playing coin sound");
        audioSource.PlayOneShot(coinPickUpSound);
        StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(coinPickUpSound.length);

        Destroy(gameObject);
    }
}

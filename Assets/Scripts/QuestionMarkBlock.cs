using System.Collections;
using UnityEngine;

public class QuestionMarkBlock : InteractableObject
{
    [SerializeField] public float bounceDuration = 0.7f;
    public CoinBlock coinBlock;
    [SerializeField] public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animator)
        {
            animator.SetBool("isActive", isActive);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator HoldBlockInPlaceCoroutine()
    {
        yield return new WaitForSeconds(bounceDuration);
        HoldBlockInPlace();
    }

    public override void Interact()
    {
        if (animator)
        {
            Debug.Log("QuestionMarkBlock interacted with");
            animator.SetBool("isActive", isActive);
        }
        else
        {
            Debug.LogWarning("Red Brick Block interacted with");
        }
        coinBlock.ReleaseCoin();
        StartCoroutine(HoldBlockInPlaceCoroutine());
    }
}

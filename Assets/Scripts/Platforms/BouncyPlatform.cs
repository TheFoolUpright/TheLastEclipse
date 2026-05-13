using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 4f;
    [SerializeField] private bool resetDoubleJump = true;

    public float BounceHeight => bounceHeight;
    public bool ResetDoubleJump => resetDoubleJump;

    [Header("Audio")]
    [SerializeField] private string bounceSoundName = "Bounce";

    public void PlayBounceFeedback()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(bounceSoundName);
        }
    }
}
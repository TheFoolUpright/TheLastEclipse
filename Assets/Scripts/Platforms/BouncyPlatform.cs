using StarterAssets;
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 4f;
    [SerializeField] private bool resetDoubleJump = true;

    // These public properties let other scripts READ the bounce settings
    // without directly changing the private fields.
    public float BounceHeight => bounceHeight;
    public bool ResetDoubleJump => resetDoubleJump;

    public AudioClip bounceAudioClip;
    [Range(0, 1)] public float bounceAudioVolume = 0.5f;

    public void PlayBounceFeedback()
    {
        // This method is where we play the platform's "bounce feedback."
        // Right now that is just a sound, but later you could also add:
        // - a squash animation
        // - particles
        // - camera shake
        AudioSource.PlayClipAtPoint(bounceAudioClip, transform.position, bounceAudioVolume);
    }

}

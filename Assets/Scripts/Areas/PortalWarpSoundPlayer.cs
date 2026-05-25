using UnityEngine;

public class PortalWarpSoundPlayer : MonoBehaviour
{
    [SerializeField] private string warpSFXName = "WarpPortal";

    private void Start()
    {
        if (!PortalAudioData.playWarpSoundOnSceneLoad)
            return;

        PortalAudioData.playWarpSoundOnSceneLoad = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(warpSFXName);
        }
    }
}
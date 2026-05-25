using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnPointName;

    [Header("Audio")]
    [SerializeField] private AudioSource portalLoopSource;
    [SerializeField] private string portalEnterSFX = "PortalEnter";
    [SerializeField] private float sceneLoadDelay = 0.5f;

    private bool isActivated = false;
    private bool hasTriggered = false;

    public void ActivatePortal()
    {
        Debug.Log("Portal activated");

        if (isActivated)
            return;

        isActivated = true;

        if (portalLoopSource != null)
        {
            Debug.Log("Playing portal loop");
            portalLoopSource.loop = true;
            portalLoopSource.Play();
        }
        else
        {
            Debug.LogWarning("Portal loop source is missing");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
            return;

        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        PortalAudioData.playWarpSoundOnSceneLoad = true;
        PortalSpawnData.spawnPointName = spawnPointName;

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator EnterPortalRoutine()
    {
        if (AudioManager.Instance != null)
        {
            PortalAudioData.playWarpSoundOnSceneLoad = true;
            SceneManager.LoadScene(sceneToLoad);
        }

        PortalSpawnData.spawnPointName = spawnPointName;

        yield return new WaitForSeconds(sceneLoadDelay);

        SceneManager.LoadScene(sceneToLoad);
    }
}
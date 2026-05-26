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

    [SerializeField] private bool isActivated = true;
    private bool hasTriggered = false;

    private void Start()
    {
        if (isActivated && portalLoopSource != null)
        {
            portalLoopSource.loop = true;
            portalLoopSource.Play();
        }
    }

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

        StartCoroutine(EnterPortalRoutine());
    }

    private IEnumerator EnterPortalRoutine()
    {
        PortalAudioData.playWarpSoundOnSceneLoad = true;
        PortalSpawnData.spawnPointName = spawnPointName;

        yield return new WaitForSeconds(sceneLoadDelay);

        SceneManager.LoadScene(sceneToLoad);
    }
}
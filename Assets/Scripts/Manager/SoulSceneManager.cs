using UnityEngine;

public class SoulSceneManager : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private GameObject returnPortal;

    private bool mainSoulCollected;

    private void Start()
    {
        if (returnPortal != null)
            returnPortal.SetActive(false);
    }

    public void CollectMainSoul()
    {
        if (mainSoulCollected)
            return;

        mainSoulCollected = true;

        if (returnPortal != null)
            returnPortal.SetActive(true);

        Debug.Log("Main soul collected. Return portal opened.");
    }
}
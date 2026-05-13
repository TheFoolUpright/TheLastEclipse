using UnityEngine;

public enum MainSoulType
{
    Flee,
    Attack
}

public class SoulSceneManager : MonoBehaviour
{
    [Header("Soul Settings")]
    [SerializeField] private MainSoulType mainSoulType;

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

        SoulUI soulUI = FindAnyObjectByType<SoulUI>();

        if (soulUI != null)
        {
            if (mainSoulType == MainSoulType.Flee)
            {
                soulUI.SetFleeCollected();
            }
            else if (mainSoulType == MainSoulType.Attack)
            {
                soulUI.SetAttackCollected();
            }
        }

        if (returnPortal != null)
            returnPortal.SetActive(true);

        Debug.Log("Main soul collected. Return portal opened.");
    }
}
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

        if (mainSoulType == MainSoulType.Flee)
        {
            GameProgress.fleeSoulCollected = true;

            if (soulUI != null)
                soulUI.SetFleeCollected();
        }
        else if (mainSoulType == MainSoulType.Attack)
        {
            GameProgress.attackSoulCollected = true;

            if (soulUI != null)
                soulUI.SetAttackCollected();
        }

        if (returnPortal != null)
            returnPortal.SetActive(true);

        Debug.Log("Main soul collected. Return portal opened.");
    }
}
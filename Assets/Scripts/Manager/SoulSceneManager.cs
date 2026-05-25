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

        if (mainSoulType == MainSoulType.Flee)
        {
            GameProgress.fleeSoulCollected = true;
        }
        else if (mainSoulType == MainSoulType.Attack)
        {
            GameProgress.attackSoulCollected = true;
        }

        if (returnPortal != null)
        {
            returnPortal.SetActive(true);

            Portal portal = returnPortal.GetComponent<Portal>();
            if (portal != null)
            {
                portal.ActivatePortal();
            }
        }
    
        Debug.Log("Main soul collected. Return portal opened.");
    }

    public void CollectSoul(GameObject soulObject)
    {
        SoulCollectibleData soulData = soulObject.GetComponent<SoulCollectibleData>();

        if (soulData == null)
            return;

        if (SoulCollectionManager.Instance != null)
        {
            SoulCollectionManager.Instance.CollectSoul(soulData.soulID);
        }

        if (soulData.soulType == SoulType.Flee || soulData.soulType == SoulType.Attack)
        {
            CollectMainSoul();
        }
    }
}
using UnityEngine;

public class HubProgressManager : MonoBehaviour
{
    [SerializeField] private GameObject finalPortal;

    [Header("Main Soul IDs")]
    [SerializeField] private string fleeSoulID;
    [SerializeField] private string attackSoulID;

    private void Start()
    {
        RefreshFinalPortal();
    }

    public void RefreshFinalPortal()
    {
        if (finalPortal == null)
            return;

        finalPortal.SetActive(GameProgress.BothMainSoulsCollected);
    }

    public void CheatCollectAllMainSouls()
    {
        GameProgress.fleeSoulCollected = true;
        GameProgress.attackSoulCollected = true;

        if (SoulCollectionManager.Instance != null)
        {
            SoulCollectionManager.Instance.CollectSoul(fleeSoulID);
            SoulCollectionManager.Instance.CollectSoul(attackSoulID);
        }

        RefreshFinalPortal();

        Debug.Log("CHEAT: All main souls collected. Final portal activated.");
    }
}
using UnityEngine;

public class HideIfSoulCollected : MonoBehaviour
{
    [SerializeField] private SoulCollectibleData soulData;

    private void Awake()
    {
        if (soulData == null)
            soulData = GetComponent<SoulCollectibleData>();
    }

    private void Start()
    {
        if (soulData == null)
            return;

        if (SoulCollectionManager.Instance != null &&
            SoulCollectionManager.Instance.IsSoulCollected(soulData.soulID))
        {
            gameObject.SetActive(false);
        }
    }
}
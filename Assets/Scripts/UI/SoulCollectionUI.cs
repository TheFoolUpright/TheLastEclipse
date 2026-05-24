using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulCollectionUI : MonoBehaviour
{
    [Header("Soul List")]
    [SerializeField] private List<SoulCollectionUIEntry> allSouls = new List<SoulCollectionUIEntry>();

    [Header("UI")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Image slotPrefab;
    [SerializeField] private Sprite uncollectedSprite;

    private readonly List<Image> spawnedSlots = new List<Image>();

    private void Start()
    {
        BuildSlots();
        RefreshUI();

        if (SoulCollectionManager.Instance != null)
        {
            SoulCollectionManager.Instance.OnSoulCollectionChanged += RefreshUI;
        }
        else
        {
            Debug.LogWarning("SoulCollectionManager Instance was not found.");
        }
    }

    private void OnDestroy()
    {
        if (SoulCollectionManager.Instance != null)
            SoulCollectionManager.Instance.OnSoulCollectionChanged -= RefreshUI;
    }

    private void BuildSlots()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        spawnedSlots.Clear();

        for (int i = 0; i < allSouls.Count; i++)
        {
            Image newSlot = Instantiate(slotPrefab, slotContainer);
            spawnedSlots.Add(newSlot);
        }
    }

    private void RefreshUI()
    {
        Debug.Log("Refreshing Soul UI");

        for (int i = 0; i < allSouls.Count; i++)
        {
            SoulCollectionUIEntry soul = allSouls[i];
            Image slot = spawnedSlots[i];

            bool collected = SoulCollectionManager.Instance != null &&
                             SoulCollectionManager.Instance.IsSoulCollected(soul.soulID);

            slot.sprite = collected ? soul.collectedSprite : uncollectedSprite;
        }
    }
}
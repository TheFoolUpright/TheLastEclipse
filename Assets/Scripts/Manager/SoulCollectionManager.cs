using System;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollectionManager : MonoBehaviour
{
    public static SoulCollectionManager Instance { get; private set; }

    public event Action OnSoulCollectionChanged;

    private HashSet<string> collectedSoulIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CollectSoul(string soulID)
    {
        if (string.IsNullOrEmpty(soulID))
        {
            Debug.LogWarning("Tried to collect a soul with no ID.");
            return;
        }

        if (collectedSoulIDs.Add(soulID))
        {
            Debug.Log("Collected soul: " + soulID);
            OnSoulCollectionChanged?.Invoke();
        }
        else
        {
            Debug.Log("Soul already collected: " + soulID);
        }
    }

    public bool IsSoulCollected(string soulID)
    {
        return collectedSoulIDs.Contains(soulID);
    }
}
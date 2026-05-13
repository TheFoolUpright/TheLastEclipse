using UnityEngine;

public class HubProgressManager : MonoBehaviour
{
    [SerializeField] private GameObject finalPortal;

    private void Start()
    {
        if (finalPortal == null)
            return;

        finalPortal.SetActive(GameProgress.BothMainSoulsCollected);
    }
}
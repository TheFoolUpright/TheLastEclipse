using UnityEngine;

public class FleeSoulDetectionTrigger : MonoBehaviour
{
    [SerializeField] private FleeSoulAgent owner;

    private void Reset()
    {
        owner = GetComponentInParent<FleeSoulAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        owner.SetPlayerInDetectionRange(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        owner.SetPlayerInDetectionRange(false);
    }
}
using UnityEngine;

public class AttachPlayerToPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.CompareTag("Player")) return;

        other.transform.SetParent(transform.parent);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.transform.SetParent(null);


    }
}

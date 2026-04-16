using UnityEngine;

public class SoulRiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.PlayerDie();
        }
    }
}
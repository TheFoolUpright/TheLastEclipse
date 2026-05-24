using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class AttackSoulDamageAreaTrigger : MonoBehaviour
{
    AttackSoulDamageArea damageArea;

    private void Awake()
    {
        damageArea = GetComponentInParent<AttackSoulDamageArea>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                damageArea.OnPlayerTrigger(controller);

            }
        }
    }
}

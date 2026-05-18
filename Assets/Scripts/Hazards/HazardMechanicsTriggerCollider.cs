using UnityEngine;

public class HazardMechanicsTriggerCollider : MonoBehaviour
{
    private HazardsMechanics hazardsMechanics;

    private void Awake()
    {
        hazardsMechanics = GetComponentInParent<HazardsMechanics>();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                hazardsMechanics.OnPlayerTriggerStay(controller, this.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                hazardsMechanics.OnPlayerTriggerExit();
            }
        }
    }
}

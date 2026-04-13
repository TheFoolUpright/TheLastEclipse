using UnityEngine;

public class AreaPlayerDetection : MonoBehaviour
{
    public string areaName;
    private void OnTriggerEnter(Collider other)
    {

        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                TextAreaPopUp.instance.EnterArea(areaName);
            }
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class Platformcollision : MonoBehaviour
{

    [SerializeField] string playertag = "Player";
    [SerializeField] Transform platform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playertag))
        {
            other.gameObject.transform.parent = platform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playertag))
        {
            //other.gameObject.transform.parent = null;
        }

    }

}
        
       



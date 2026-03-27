using System;
using System.Collections;
using UnityEngine;

public class HazardsMechanics : MonoBehaviour
{
    public Character hazardSide;
    public PlayerController player;
    public GameObject hazardVisual;
    public GameObject neutralVisual;
    public BoxCollider onTriggerCollider;
    private void Start()
    {
        ActiveVisual();
    }
    private void OnEnable()
    {
        player.characterChanged += ActiveVisual;
    }

    private void OnDisable()
    {
        player.characterChanged -= ActiveVisual;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I was here, I collided");
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
              if ((controller.IsMoonActive && hazardSide == Character.Moon) || 
                    (!controller.IsMoonActive && hazardSide == Character.Sun))
              {
                   controller.Damage();
              }
            }
        }
    }

    private void ActiveVisual()
    {
        hazardVisual.SetActive((player.IsMoonActive && hazardSide == Character.Moon) || 
            (!player.IsMoonActive && hazardSide == Character.Sun));
        neutralVisual.SetActive((!player.IsMoonActive && hazardSide == Character.Moon) ||
            (player.IsMoonActive && hazardSide == Character.Sun));

        StartCoroutine(RefreshCollider());
    }

    private IEnumerator RefreshCollider()
    {
        onTriggerCollider.enabled = false;
        yield return null;

        onTriggerCollider.enabled = true;

    }
}



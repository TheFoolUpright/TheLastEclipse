using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HazardsMechanics : MonoBehaviour
{
    public Character hazardSide;
    public PlayerController player;
    public GameObject hazardVisual;
    public GameObject neutralVisual;
    public BoxCollider onTriggerCollider;

    private float damageCooldown;
    private float damageCooldownDuration = 3f;

    private void Start()
    {
        //ActiveVisual();
    }
    private void OnEnable()
    {
        player.OnCharacterChanged += ActiveVisual;
    }

    private void OnDisable()
    {
        player.OnCharacterChanged -= ActiveVisual;
    }
    private void OnTriggerStay(Collider other)
    {

        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {

                if ((controller.IsMoonActive && hazardSide == Character.Moon) || 
                    (!controller.IsMoonActive && hazardSide == Character.Sun))
              {
                    damageCooldown -= Time.deltaTime;
                    if (damageCooldown <= 0)
                    {
                        controller.Damage();
                        damageCooldown = damageCooldownDuration;
                    }
                    
              }
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
                damageCooldown = 0;
            }
        }
    }

    private void ActiveVisual(Character characterType)
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



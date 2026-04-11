using System;
using UnityEngine;

public class AttackSoulDamageArea : MonoBehaviour
{
    AttackSoulAgent owner;
    internal void Initialize(AttackSoulAgent attackSoulAgent)
    {
        owner = attackSoulAgent;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                owner.attackHit = true;
                controller.Damage();
                
            }
        }
    }
}

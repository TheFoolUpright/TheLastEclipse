using System;
using System.Collections;
using UnityEngine;

public class AttackSoulDamageArea : MonoBehaviour
{
    public Transform endTarget;

    AttackSoulAgent owner;
    private BoxCollider boxCollider;
    private bool initialized;
    internal void Initialize(AttackSoulAgent attackSoulAgent)
    {
        owner = attackSoulAgent;
        gameObject.SetActive(false);
        boxCollider = GetComponent<BoxCollider>();
        initialized = true;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                Debug.Log("Damaged");
                owner.attackHit = true;
                controller.Damage();
                boxCollider.enabled = false;
                
            }
        }
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            return;
        }
        StartCoroutine(ActivateCollider());
    }
    private IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(2);
        boxCollider.enabled = true;
    }
}

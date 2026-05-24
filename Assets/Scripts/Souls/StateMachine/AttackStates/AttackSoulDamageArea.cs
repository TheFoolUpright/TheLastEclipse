using System;
using System.Collections;
using UnityEngine;

public class AttackSoulDamageArea : MonoBehaviour
{
    public Transform endTarget;

    AttackSoulAgent owner;
    private MeshCollider collider;
    private bool initialized;
    internal void Initialize(AttackSoulAgent attackSoulAgent)
    {
        owner = attackSoulAgent;
        gameObject.SetActive(false);
        collider = GetComponentInChildren<MeshCollider>();
        initialized = true;
        collider.enabled = false;
    }

    public void OnPlayerTrigger(PlayerController controller)
    {
        Debug.Log("Damaged");
        owner.attackHit = true;
        controller.Damage(owner.transform.position);
        collider.enabled = false;
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
        collider.enabled = true;
    }

    internal float GetAttackDistance()
    {
        return Vector3.Distance(transform.position, endTarget.position);
    }
}

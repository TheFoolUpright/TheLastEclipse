using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private PlayerController playerController;

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnHpChanged += SetHearts;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnHpChanged -= SetHearts;
        }
    }

    private void Start()
    {
        if (playerController != null)
        {
            SetHearts(playerController.CurrentHealth);
        }
    }

    public void SetHearts(int hp)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].SetActive(i < hp);
            }
        }
    }
}
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public List<GameObject> hearts;
    public PlayerController playerController;

    private void OnEnable()
    {
        playerController.onHpChange += SetHearts;
    }

    private void OnDisable()
    {
        playerController.onHpChange -= SetHearts;
    }

    public void SetHearts(int hp)
    {
        for (int i = 0; i< hearts.Count; i++)
        {
            hearts[i].SetActive(i + 1 <= hp);
        }
    }
}

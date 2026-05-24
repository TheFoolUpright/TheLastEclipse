using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Blocks")]
    [SerializeField] private List<Image> healthBlocks;
    [SerializeField] private Sprite fullBlockSprite;
    [SerializeField] private Sprite emptyBlockSprite;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    private void OnEnable()
    {
        if (playerController != null)
            playerController.OnHpChanged += SetHealthBlocks;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.OnHpChanged -= SetHealthBlocks;
    }

    private void Start()
    {
        if (playerController != null)
            SetHealthBlocks(playerController.CurrentHealth);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public void SetHealthBlocks(int hp)
    {
        for (int i = 0; i < healthBlocks.Count; i++)
        {
            if (healthBlocks[i] == null)
                continue;

            healthBlocks[i].sprite = i < hp ? fullBlockSprite : emptyBlockSprite;
        }
    }
}
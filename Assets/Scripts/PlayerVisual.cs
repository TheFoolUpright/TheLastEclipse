using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private GameObject sunVisual;
    [SerializeField] private GameObject moonVisual;

    private PlayerInput playerInput;
    private InputAction changeVisualAction;
    private float cooldownBetweenVisuals = 0.5f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        changeVisualAction = playerInput.actions["ChangeVisual"];
    }
    private void Start()
    {
        SetVisual(true);
    }

    private void Update()
    {
        if(cooldownBetweenVisuals > 0)
        {
            cooldownBetweenVisuals -= Time.deltaTime;
        } 
    }

    private void OnEnable()
    {
        changeVisualAction.performed += ChangeVisual;
    }

    private void OnDisable()
    {
        changeVisualAction.performed -= ChangeVisual;
    }

    private void ChangeVisual(InputAction.CallbackContext context)
    {
        if (cooldownBetweenVisuals > 0)
            return;

        if (sunVisual.activeSelf)
        {
            SetVisual(false);
        }
        else
        {
            SetVisual(true);
        }
        cooldownBetweenVisuals = 0.5f;
    }

    private void SetVisual(bool isSun)
    {
        sunVisual.SetActive(isSun);
        moonVisual.SetActive(!isSun);
    }
}

using StarterAssets;
using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private StarterAssetsInputs inputs;

    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;
    [SerializeField] [Range(0.1f, 2f)] float delayBetweenChanges;
    [SerializeField] CharacterController characterController;

    private int _currentHealth = 0;
    private int currentHealth 
    {
        get => _currentHealth;
        set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                onHpChange.Invoke(_currentHealth);
            }
        }
    }
    private float timer = 0f;
    private int maxHealth = 3;
    public Action<int> onHpChange;

    private Vector3 startingPosition; 

    private void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        timer = delayBetweenChanges;
        startingPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (timer  > 0f)
        {
            timer -= Time.deltaTime;
        }

        if (inputs.changeVisual && timer <= 0)
        {
            ActiveVisual(!sunVisual.activeInHierarchy);
            timer = delayBetweenChanges;
        }

        if(transform.position.y < -10)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        currentHealth = 0;
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        characterController.enabled = false;
        transform.position = startingPosition;
        characterController.enabled = true;
        currentHealth = maxHealth;
    }

    private void ActiveVisual (bool isSun)
    {
        moonVisual.SetActive(!isSun);
        sunVisual.SetActive(isSun);
    }

}

using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public event Action<Character> OnCharacterChanged;

    public Character CurrentCharacter => moonVisual.activeInHierarchy ? Character.Moon : Character.Sun;
    public bool IsMoonActive => moonVisual.activeInHierarchy;

    private List<Color> originalColors = new List<Color>();
    private bool isDamaged;
    private SkinnedMeshRenderer moonRenderer;
    private SkinnedMeshRenderer sunRenderer;

    private StarterAssetsInputs inputs;

    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;
    [SerializeField] private GameObject sunUISymbol;
    [SerializeField] private GameObject moonUISymbol;
    [SerializeField] [Range(0.1f, 2f)] float delayBetweenChanges;
    [SerializeField] CharacterController characterController;
    [SerializeField] private int _currentHealth = 0;

    private int currentHealth 
    {
        get => _currentHealth;
        set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                onHpChange?.Invoke(_currentHealth);
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
        moonRenderer = moonVisual.GetComponentInChildren<SkinnedMeshRenderer>();
        sunRenderer = sunVisual.GetComponentInChildren<SkinnedMeshRenderer>();
        sunUISymbol.SetActive(true);
        moonUISymbol.SetActive(false);

    }

    private void Update()
    {
        if (timer  > 0f)
        {
            timer -= Time.deltaTime;
        }

        if (inputs.changeVisual && timer <= 0 && !isDamaged)
        {
            ActiveVisual(IsMoonActive);
            timer = delayBetweenChanges;
        }

        if(transform.position.y < -10)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
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
        moonUISymbol.SetActive(!isSun);
        sunUISymbol.SetActive(isSun);

        OnCharacterChanged?.Invoke(moonVisual.activeInHierarchy ? Character.Moon : Character.Sun);
    }

    internal void Damage()
    {
        currentHealth--;
        
        if (currentHealth <= 0)
        {
            RespawnPlayer();

        } 
        else
        {
            originalColors = new();
            for(int i = 0; i < 3; i++)
            {
                if (IsMoonActive)
                {
                    originalColors.Add(moonRenderer.materials[i].color);
                } else
                {
                    originalColors.Add(sunRenderer.materials[i].color);
                }
            }
            isDamaged = true;
            StartCoroutine(DamagedEffect());
        }
    }
    public void SetStateColor(Color color)
    {
        SkinnedMeshRenderer renderer = IsMoonActive ? moonRenderer : sunRenderer;
        for (int i = 0; i < 3; i++)
        {
            renderer.materials[i].color = color;
        }
    }

    public void ResetColor()
    {
        SkinnedMeshRenderer renderer = IsMoonActive ? moonRenderer : sunRenderer;
        for(int i = 0; i < 3; i++)
        {
            renderer.materials[i].color = originalColors[i];
        }
    }

    private IEnumerator DamagedEffect()
    {
        SetStateColor(Color.red);
        yield return new WaitForSeconds(1);
        ResetColor();
        isDamaged = false;
    }
}

public enum Character
{
    Sun, 
    Moon
}
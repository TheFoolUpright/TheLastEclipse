using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action<Character> OnCharacterChanged;
    public event Action<int> OnHpChanged;

    public Character CurrentCharacter => moonVisual.activeSelf ? Character.Moon : Character.Sun;
    public bool IsMoonActive => CurrentCharacter == Character.Moon;

    [Header("Character Visuals")]
    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;

    [Header("UI")]
    [SerializeField] private GameObject sunUISymbol;
    [SerializeField] private GameObject moonUISymbol;

    [Header("References")]
    [SerializeField] private CharacterController characterController;

    [Header("Settings")]
    [SerializeField, Range(0.1f, 2f)] private float delayBetweenChanges = 0.5f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private float damageFlashDuration = 1f;
    [SerializeField] private Color damageColor = Color.red;

    private StarterAssetsInputs inputs;
    private SkinnedMeshRenderer moonRenderer;
    private SkinnedMeshRenderer sunRenderer;

    private readonly List<Color> moonOriginalColors = new();
    private readonly List<Color> sunOriginalColors = new();

    private float switchTimer;
    private bool isDamaged;
    private int currentHealth;
    private Vector3 startingPosition;
    private Coroutine damageRoutine;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        startingPosition = transform.position;
        switchTimer = 0f;
        currentHealth = maxHealth;

        moonRenderer = moonVisual.GetComponentInChildren<SkinnedMeshRenderer>();
        sunRenderer = sunVisual.GetComponentInChildren<SkinnedMeshRenderer>();

        CacheOriginalColors(moonRenderer, moonOriginalColors);
        CacheOriginalColors(sunRenderer, sunOriginalColors);

        SetCharacter(Character.Sun, notify: false);
        OnHpChanged?.Invoke(currentHealth);
    }

    private void Update()
    {
        UpdateSwitchTimer();
        HandleCharacterSwitchInput();
        CheckFallDeath();
    }

    private void UpdateSwitchTimer()
    {
        if (switchTimer > 0f)
        {
            switchTimer -= Time.deltaTime;
        }
    }

    private void HandleCharacterSwitchInput()
    {
        if (!inputs.changeVisual || switchTimer > 0f || isDamaged)
            return;

        inputs.changeVisual = false;
        ToggleCharacter();
        switchTimer = delayBetweenChanges;
    }

    private void CheckFallDeath()
    {
        if (transform.position.y < fallDeathY)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        currentHealth = 0;
        OnHpChanged?.Invoke(currentHealth);
        RespawnPlayer();
    }

    public void Damage()
    {
        if (isDamaged)
            return;

        currentHealth--;
        OnHpChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            RespawnPlayer();
            return;
        }

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        damageRoutine = StartCoroutine(DamagedEffect());
    }

    private void RespawnPlayer()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }

        isDamaged = false;
        ResetActiveCharacterColor();

        characterController.enabled = false;
        transform.position = startingPosition;
        characterController.enabled = true;

        currentHealth = maxHealth;
        OnHpChanged?.Invoke(currentHealth);
    }

    private void ToggleCharacter()
    {
        Character nextCharacter = IsMoonActive ? Character.Sun : Character.Moon;
        SetCharacter(nextCharacter, notify: true);
    }

    private void SetCharacter(Character character, bool notify)
    {
        bool isSun = character == Character.Sun;

        sunVisual.SetActive(isSun);
        moonVisual.SetActive(!isSun);

        sunUISymbol.SetActive(isSun);
        moonUISymbol.SetActive(!isSun);

        if (notify)
        {
            OnCharacterChanged?.Invoke(character);
        }
    }

    private IEnumerator DamagedEffect()
    {
        isDamaged = true;

        SetActiveCharacterColor(damageColor);
        yield return new WaitForSeconds(damageFlashDuration);
        ResetActiveCharacterColor();

        isDamaged = false;
        damageRoutine = null;
    }

    private void SetActiveCharacterColor(Color color)
    {
        SkinnedMeshRenderer renderer = GetActiveRenderer();
        if (renderer == null) return;

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = color;
        }
    }

    private void ResetActiveCharacterColor()
    {
        SkinnedMeshRenderer renderer = GetActiveRenderer();
        List<Color> originalColors = GetActiveOriginalColors();

        if (renderer == null || originalColors == null) return;

        Material[] materials = renderer.materials;
        int count = Mathf.Min(materials.Length, originalColors.Count);

        for (int i = 0; i < count; i++)
        {
            materials[i].color = originalColors[i];
        }
    }

    private SkinnedMeshRenderer GetActiveRenderer()
    {
        return IsMoonActive ? moonRenderer : sunRenderer;
    }

    private List<Color> GetActiveOriginalColors()
    {
        return IsMoonActive ? moonOriginalColors : sunOriginalColors;
    }

    private void CacheOriginalColors(SkinnedMeshRenderer renderer, List<Color> colorCache)
    {
        colorCache.Clear();

        if (renderer == null)
        {
            Debug.LogError("PlayerController: Missing SkinnedMeshRenderer.");
            return;
        }

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            colorCache.Add(materials[i].color);
        }
    }
}

public enum Character
{
    Sun,
    Moon
}
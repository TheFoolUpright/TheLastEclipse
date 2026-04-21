using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Color = UnityEngine.Color;

public class PlayerController : MonoBehaviour
{
    public event Action<Character> OnCharacterChanged;
    public event Action<int> OnHpChanged;

    public Character CurrentCharacter => moonVisual.activeSelf ? Character.Moon : Character.Sun;
    public bool IsMoonActive => CurrentCharacter == Character.Moon;
    private Material activeMaterial => IsMoonActive ? moonMaterial : sunMaterial; 

    [Header("Character Visuals")]
    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;

    [Header("UI")]
    [SerializeField] private GameObject sunUISymbol;
    [SerializeField] private GameObject moonUISymbol;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Material sunMaterial;
    [SerializeField] private Material moonMaterial;


    [Header("Settings")]
    [SerializeField, Range(0.1f, 2f)] private float delayBetweenChanges = 0.5f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private float damageFlashDuration = 1f;

    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color moonColor = Color.blue;
    [SerializeField] private Color sunColor = Color.yellow;

    private StarterAssetsInputs inputs;

    private float switchTimer;
    private bool isDamaged;
    private int currentHealth;
    [SerializeField] private Vector3 startingPosition;
    private Coroutine damageRoutine;
    private float dissolveTimer;
    private float dissolveDuration = 0.25f;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        startingPosition = transform.position;
        switchTimer = 0f;
        currentHealth = maxHealth;

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
        StartCoroutine(DissolveEffect(character, notify));
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

    private IEnumerator DissolveEffect(Character character, bool notify)
    {
        bool isSun = character == Character.Sun;

        dissolveTimer = 0;

        while (dissolveTimer < dissolveDuration)
        {
            dissolveTimer += Time.deltaTime;
            activeMaterial.SetFloat("_DissolveForce", dissolveTimer / dissolveDuration);
            yield return null;
        }

        sunVisual.SetActive(isSun);
        moonVisual.SetActive(!isSun);
        dissolveTimer = 0;

        while (dissolveTimer < dissolveDuration)
        {
            dissolveTimer += Time.deltaTime;
            activeMaterial.SetFloat("_DissolveForce", 1 - (dissolveTimer / dissolveDuration));
            yield return null;
        }
        sunUISymbol.SetActive(isSun);
        moonUISymbol.SetActive(!isSun);

        ResetActiveCharacterColor();

        if (notify)
        {
            OnCharacterChanged?.Invoke(character);
        }
    }

    private void SetActiveCharacterColor(Color color)
    {
        activeMaterial.SetColor("_Base_Color", color);
    }

    private void ResetActiveCharacterColor()
    {
        activeMaterial.SetColor("_Base_Color", IsMoonActive ? moonColor : sunColor);
        activeMaterial.SetFloat("_DissolveForce", 0);
    }
}

public enum Character
{
    Sun,
    Moon
}
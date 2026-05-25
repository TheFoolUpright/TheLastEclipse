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

    [SerializeField] private Character currentCharacter = Character.Sun;

    public Character CurrentCharacter => currentCharacter;
    public bool IsMoonActive => currentCharacter == Character.Moon;

    private List<Material> activeMaterials => IsMoonActive ? moonMaterials : sunMaterials;

    [Header("Character Visuals")]
    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private List<Material> sunMaterials;
    [SerializeField] private List<Material> moonMaterials;
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private GameObject sunDecor;
    [SerializeField] private GameObject moonDecor;
    [SerializeField] private GameObject hurtDecor;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int animIDHurt;
    private int animIDDead;

    [Header("Audio")]
    [SerializeField] private string hurtSoundName = "Hurt";
    [SerializeField] private string switchSoundName = "CharacterSwitch";
    [SerializeField] private string jumpSoundName = "Jump";

    [Header("Settings")]
    [SerializeField, Range(0.1f, 2f)] private float delayBetweenChanges = 0.5f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float fallDeathY = -15f;
    [SerializeField] private float damageFlashDuration = 1f;
    private float dissolveDuration => delayBetweenChanges / 2;

    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color moonColor = Color.blue;
    [SerializeField] private Color sunColor = Color.yellow;

    private StarterAssetsInputs inputs;

    private float switchTimer;
    private float fallingTimer;
    public float fallingThreshold = 5f;
    private bool isDamaged;
    private int currentHealth;
    private Coroutine damageRoutine;
    private float dissolveTimer;

    private bool dissolveEnabled = false;

    [SerializeField] private Vector3 respawnPosition;
    [SerializeField] private Quaternion respawnRotation;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        animIDHurt = Animator.StringToHash("IsHurt");
        animIDDead = Animator.StringToHash("IsDead");

        SetRespawnPoint(defaultSpawnPoint);

        sunDecor.SetActive(false);
        moonDecor.SetActive(false);

        switchTimer = 0f;
        currentHealth = maxHealth;
        SetCharacterImmediate(Character.Sun, notify: true);

        OnHpChanged?.Invoke(currentHealth);

    }

    private void Start()
    {
        TryMoveToPortalSpawnPoint();
    }


    private void SetRespawnPoint(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("No spawn point assigned. Using player position instead.");

            respawnPosition = transform.position;
            respawnRotation = transform.rotation;
            return;
        }

        respawnPosition = spawnPoint.position;
        respawnRotation = spawnPoint.rotation;
    }

    private void TryMoveToPortalSpawnPoint()
    {
        if (string.IsNullOrEmpty(PortalSpawnData.spawnPointName))
            return;

        GameObject spawnPoint = GameObject.Find(PortalSpawnData.spawnPointName);

        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point not found: " + PortalSpawnData.spawnPointName);
            return;
        }

        characterController.enabled = false;

        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;

        characterController.enabled = true;

        SetRespawnPoint(spawnPoint.transform);

        PortalSpawnData.spawnPointName = null;
    }

    public void CheatRefillHealth()
    {
        currentHealth = maxHealth;
        OnHpChanged?.Invoke(currentHealth);
    }

    public void CheatRespawn()
    {
        RespawnPlayer();
    }

    private void Update()
    {
        UpdateSwitchTimer();
        HandleCharacterSwitchInput();
        CheckPlayerFalling();
    }

    private void CheckPlayerFalling()
    {
        if (!characterController.isGrounded && characterController.velocity.y < 0)
        {
            fallingTimer += Time.deltaTime;
            if (fallingTimer >= fallingThreshold)
            {
                PlayerDie();
                fallingTimer = 0;
            }
        }
        else
        {
            fallingTimer = 0;
        }
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
        PlaySwitchFeedback();
        switchTimer = delayBetweenChanges;
    }

    public void PlaySwitchFeedback()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(switchSoundName);
        }
    }

    private void SetCharacterImmediate(Character character, bool notify)
    {
        currentCharacter = character;

        bool isSun = character == Character.Sun;

        sunVisual.SetActive(isSun);
        moonVisual.SetActive(!isSun);


        ResetActiveCharacterColor();

        if (notify)
            OnCharacterChanged?.Invoke(currentCharacter);
    }

    public void PlayerDie()
    {
        currentHealth = 0;
        OnHpChanged?.Invoke(currentHealth);

        if (animator != null)
        {
            animator.SetTrigger(animIDDead);
        }

        AudioManager.Instance.PlaySFX("Death");

        StartCoroutine(RespawnAfterDeathAnimation());
    }
    private IEnumerator RespawnAfterDeathAnimation()
    {
        yield return new WaitForSeconds(1.0f);
        RespawnPlayer();
    }

    public void Damage(Vector3 damageSourcePosition)
    {
        if (isDamaged)
            return;

        currentHealth--;
        PlayHurtFeedback();
        OnHpChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            PlayerDie();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger(animIDHurt);
        }

        Knockback(damageSourcePosition);

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        damageRoutine = StartCoroutine(DamagedEffect());
    }

    public void PlayHurtFeedback()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hurtSoundName);
        }
    }

    private void Knockback(Vector3 damageSourcePosition)
    {
        Vector3 direction = (this.transform.position - damageSourcePosition).normalized;
        StartCoroutine(KnockbackCourotine(direction));
    }

    private IEnumerator KnockbackCourotine(Vector3 direction)
    {
        float elapsed = 0f;
        float duration = 0.25f;
        float forcePower = 30;
        while (elapsed < duration)
        {
            float force = Mathf.Lerp(forcePower, 0f, elapsed / duration);
            characterController.Move(direction * force * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
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
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        characterController.enabled = true;

        currentHealth = maxHealth;
        OnHpChanged?.Invoke(currentHealth);
    }

    private void ToggleCharacter()
    {
        Character nextCharacter = IsMoonActive ? Character.Sun : Character.Moon;

        Debug.Log($"PLAYER ToggleCharacter | Current: {currentCharacter} | Next: {nextCharacter}");

        SetCharacter(nextCharacter, notify: true);
    }

    private void SetCharacter(Character character, bool notify)
    {
        StartCoroutine(DissolveEffect(character, notify));
    }

    private IEnumerator DamagedEffect()
    {
        isDamaged = true;

        //SetActiveCharacterColor(damageColor);
        hurtDecor.SetActive(true);
        yield return new WaitForSeconds(damageFlashDuration);
        hurtDecor.SetActive(false);
        ResetActiveCharacterColor();

        isDamaged = false;
        damageRoutine = null;
    }

    private IEnumerator DissolveEffect(Character character, bool notify)
    {
        Debug.Log($"PLAYER Dissolve START | Current: {currentCharacter} | Target: {character}");

        bool isSun = character == Character.Sun;

        dissolveTimer = 0;

        if (dissolveEnabled)
        {
            while (dissolveTimer < dissolveDuration)
            {
                dissolveTimer += Time.deltaTime;
                SetDissolveEffect(dissolveTimer / dissolveDuration);
                yield return null;
            }

            currentCharacter = character;

            Debug.Log($"PLAYER Character SET | Current is now: {currentCharacter}");


            dissolveTimer = 0;

            while (dissolveTimer < dissolveDuration)
            {
                dissolveTimer += Time.deltaTime;
                SetDissolveEffect(1 - (dissolveTimer / dissolveDuration));
                yield return null;
            }
        }
        else
        {
            sunDecor.SetActive(isSun);
            moonDecor.SetActive(!isSun);
            yield return new WaitForSeconds(delayBetweenChanges);
            sunVisual.SetActive(isSun);
            moonVisual.SetActive(!isSun);
            sunDecor.SetActive(false);
            moonDecor.SetActive(false);
            currentCharacter = character;
        }


        ResetActiveCharacterColor();

        if (notify)
        {
            Debug.Log($"PLAYER Notify Platforms | Sending: {currentCharacter}");
            OnCharacterChanged?.Invoke(currentCharacter);
        }
    }

    private void SetActiveCharacterColor(Color color)
    {
        //activeMaterials.SetColor("_Base_Color", color);
    }

    private void ResetActiveCharacterColor()
    {
        //activeMaterials.SetColor("_Base_Color", IsMoonActive ? moonColor : sunColor);
        SetDissolveEffect(0);
    }

    private void SetDissolveEffect(float percentage)
    {
        foreach (Material mat in activeMaterials)
        {
            mat.SetFloat("_DissolveForce", percentage);
        }

    }

}

public enum Character
{
    Sun,
    Moon
}
using UnityEngine;

public class TogglePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private Collider _platformCollider;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("Platform Settings")]
    [SerializeField] private Character _platformType;
    [SerializeField] private float _solidDuration = 2f;
    [SerializeField] private float _offDuration = 2f;
    [SerializeField] private float _startOffset = 0f;

    [Header("Visual Tints")]
    [SerializeField] private Color _solidUsableTint = Color.white;
    [SerializeField] private Color _solidBlockedTint = new Color(1f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color _offTint = new Color(1f, 1f, 1f, 0.25f);

    [Header("Blink Warning")]
    [SerializeField] private bool _useBlinkWarning = true;
    [SerializeField] private float _warningDuration = 0.5f;
    [SerializeField] private float _blinkSpeed = 8f;
    [SerializeField] private Color _warningTint = new Color(1f, 0.8f, 0.8f, 0.6f);

    private float _timer;
    private bool _isSolidPhase;
    private Character _currentCharacter;

    private Material _platformMaterial;
    private Color _originalColor = Color.white;

    private void Awake()
    {
        if (_meshRenderer != null)
        {
            _platformMaterial = _meshRenderer.material;

            if (_platformMaterial.HasProperty("_BaseColor"))
                _originalColor = _platformMaterial.GetColor("_BaseColor");
            else if (_platformMaterial.HasProperty("_Color"))
                _originalColor = _platformMaterial.GetColor("_Color");
        }

        if (_player != null)
        {
            _player.OnCharacterChanged += PlayerStateChanged;
            PlayerStateChanged(_player.CurrentCharacter);
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnCharacterChanged -= PlayerStateChanged;
        }
    }

    private void Start()
    {
        _isSolidPhase = true;
        _timer = _solidDuration;

        if (_startOffset > 0f)
        {
            ApplyStartOffset(_startOffset);
        }

        UpdatePlatformState();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            TogglePhase();
            return;
        }

        UpdateVisuals();
    }

    private void ApplyStartOffset(float offset)
    {
        while (offset > 0f)
        {
            if (_isSolidPhase)
            {
                if (offset >= _timer)
                {
                    offset -= _timer;
                    _isSolidPhase = false;
                    _timer = _offDuration;
                }
                else
                {
                    _timer -= offset;
                    offset = 0f;
                }
            }
            else
            {
                if (offset >= _timer)
                {
                    offset -= _timer;
                    _isSolidPhase = true;
                    _timer = _solidDuration;
                }
                else
                {
                    _timer -= offset;
                    offset = 0f;
                }
            }
        }
    }

    private void TogglePhase()
    {
        _isSolidPhase = !_isSolidPhase;
        _timer = _isSolidPhase ? _solidDuration : _offDuration;
        UpdatePlatformState();
    }

    private void PlayerStateChanged(Character character)
    {
        _currentCharacter = character;
        UpdatePlatformState();
    }

    private void UpdatePlatformState()
    {
        bool correctCharacter = _currentCharacter == _platformType;
        bool shouldBeSolid = _isSolidPhase && correctCharacter;

        if (_platformCollider != null)
        {
            _platformCollider.enabled = shouldBeSolid;
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_platformMaterial == null)
            return;

        bool correctCharacter = _currentCharacter == _platformType;

        if (!_isSolidPhase)
        {
            SetMaterialTint(_offTint);
            return;
        }

        Color baseTint = correctCharacter ? _solidUsableTint : _solidBlockedTint;

        bool isInWarningWindow = _useBlinkWarning && correctCharacter && _timer <= _warningDuration;

        if (isInWarningWindow)
        {
            float pulse = Mathf.PingPong(Time.time * _blinkSpeed, 1f);
            Color blinkTint = Color.Lerp(baseTint, _warningTint, pulse);
            SetMaterialTint(blinkTint);
        }
        else
        {
            SetMaterialTint(baseTint);
        }
    }

    private void SetMaterialTint(Color tint)
    {
        if (_platformMaterial == null)
            return;

        Color finalColor = new Color(
            _originalColor.r * tint.r,
            _originalColor.g * tint.g,
            _originalColor.b * tint.b,
            _originalColor.a * tint.a
        );

        if (_platformMaterial.HasProperty("_BaseColor"))
            _platformMaterial.SetColor("_BaseColor", finalColor);
        else if (_platformMaterial.HasProperty("_Color"))
            _platformMaterial.SetColor("_Color", finalColor);
    }
}
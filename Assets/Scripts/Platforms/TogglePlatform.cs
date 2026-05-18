using UnityEngine;

[System.Serializable]
public class LinkedRendererSet
{
    public MeshRenderer renderer;
    public Material opaqueMaterial;
    public Material transparentMaterial;
}

public class TogglePlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private Collider _platformCollider;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("Materials")]
    [SerializeField] private Material _opaqueMaterial;
    [SerializeField] private Material _transparentMaterial;

    [Header("Platform Settings")]
    [SerializeField] private Character _platformType;
    [SerializeField] private float _solidDuration = 2f;
    [SerializeField] private float _offDuration = 2f;
    [SerializeField] private float _startOffset = 0f;

    [Header("Blink Warning")]
    [SerializeField] private bool _useBlinkWarning = true;
    [SerializeField] private float _warningDuration = 0.5f;
    [SerializeField] private float _blinkSpeed = 8f;

    [Header("Linked Visual Objects")]
    [SerializeField] private LinkedRendererSet[] _linkedRenderers;

    private float _timer;
    private bool _isSolidPhase;
    private Character _currentCharacter;


    private void Awake()
    {
        if (_player != null)
        {
            _player.OnCharacterChanged += PlayerStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.OnCharacterChanged -= PlayerStateChanged;
    }

    private void Start()
    {
        _currentCharacter = _player.CurrentCharacter;

        _isSolidPhase = true;
        _timer = _solidDuration;

        if (_startOffset > 0f)
            ApplyStartOffset(_startOffset);

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
            if (offset >= _timer)
            {
                offset -= _timer;
                _isSolidPhase = !_isSolidPhase;
                _timer = _isSolidPhase ? _solidDuration : _offDuration;
            }
            else
            {
                _timer -= offset;
                offset = 0f;
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
        Debug.Log($"{name} PLATFORM received character change: {character}");

        _currentCharacter = character;
        UpdatePlatformState();
    }
    private void UpdatePlatformState()
    {
        bool correctCharacter = _currentCharacter == _platformType;
        bool shouldBeSolid = _isSolidPhase && correctCharacter;

        if (_platformCollider != null)
            _platformCollider.enabled = shouldBeSolid;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_meshRenderer == null)
            return;

        bool correctCharacter = _currentCharacter == _platformType;

        // Wrong character
        if (!correctCharacter)
        {
            SetAllMaterials(false);
            return;
        }

        // Off phase
        if (!_isSolidPhase)
        {
            SetAllMaterials(false);
            return;
        }

        bool isInWarningWindow =
            _useBlinkWarning &&
            _timer <= _warningDuration;

        if (isInWarningWindow)
        {
            bool useOpaque =
                Mathf.FloorToInt(Time.time * _blinkSpeed) % 2 == 0;

            SetAllMaterials(useOpaque);
        }
        else
        {
            SetAllMaterials(true);
        }
    }

    private void SetAllMaterials(bool useOpaque)
    {
        // Main platform
        if (_meshRenderer != null)
        {
            _meshRenderer.material =
                useOpaque ? _opaqueMaterial : _transparentMaterial;
        }

        // Linked objects
        if (_linkedRenderers == null)
            return;

        foreach (LinkedRendererSet linked in _linkedRenderers)
        {
            if (linked.renderer == null)
                continue;

            linked.renderer.material =
                useOpaque
                ? linked.opaqueMaterial
                : linked.transparentMaterial;
        }
    }


}
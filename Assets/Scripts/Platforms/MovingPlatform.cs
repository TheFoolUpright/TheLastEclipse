using System;
using Unity.VisualScripting;
using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private WaypointPath _waypointPath;
    [SerializeField]
    private float _speed;
    [SerializeField] private float _waitTimeAtWaypoint = 0.7f;
    [SerializeField] private float _decelerationTime = 1f; // Hoe lang het duurt om te stoppen
    private int _targetWaypointIndex;
    private Transform _previousWaypoint;
    private Transform _targetWaypoint;
    private Transform player;
    private float _timeToWaypoint;
    private float _elapsedTime;
    private float _waitTimer;
    private bool _isWaiting;
    private int _direction = 1;
    private float _speedMultiplier = 1f; // 1 = volle snelheid, 0 = gestopt
    public PlayerController Player;
    private bool isMoving;
    private bool playerOnPlat;
    [SerializeField]
    private Character _platformType;
    private void Awake()
    {
        Player.OnCharacterChanged += PlayerStateChanged;
        PlayerStateChanged(Player.CurrentCharacter);
    }
    private void OnDestroy()
    {
        Player.OnCharacterChanged -= PlayerStateChanged;
    }
    private void PlayerStateChanged(Character character)
    {
        if (character == _platformType)
            isMoving = true;
        else
            isMoving = false;
    }
    void Start()
    {
        PlayerStateChanged(Player.CurrentCharacter);
        TargetNextWaypoint();
    }
    private void FixedUpdate()
    {
        
        if (isMoving)
            _speedMultiplier = Mathf.MoveTowards(_speedMultiplier, 1f, Time.fixedDeltaTime / _decelerationTime);
        else
            _speedMultiplier = Mathf.MoveTowards(_speedMultiplier, 0.5f, Time.fixedDeltaTime / _decelerationTime);

        if (_isWaiting)
        {
            _waitTimer += Time.fixedDeltaTime;
            if (_waitTimer >= _waitTimeAtWaypoint)
            {
                _isWaiting = false;
                _waitTimer = 0f;
                TargetNextWaypoint();
            }
            return;
        }

        _elapsedTime += Time.fixedDeltaTime * _speedMultiplier; 
        float elapsedPercentage = _elapsedTime / _timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);
        if (elapsedPercentage >= 1)
        {
            _isWaiting = true;
        }
    }
    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex += _direction;
        int lastIndex = _waypointPath.transform.childCount - 1;
        if (_targetWaypointIndex == 0 || _targetWaypointIndex == lastIndex)
        {
            _direction *= -1;
        }
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _elapsedTime = 0;
        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }
    private void SetPlayerParent()
    {
        Debug.Log("Are u");
        player.SetParent(this.transform, true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (controller)
            {
                player = controller.transform;
                SetPlayerParent();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            player.parent = null;
        }
    }
}
using System;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField]
    private WaypointPath _waypointPath;

    [SerializeField]
    private float _speed;

    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    private int _direction = 1;

    public PlayerController Player;
    private bool isMoving;
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

   
    void Update()
    {
        if (isMoving)
        {
        _elapsedTime += Time.deltaTime;

            float elapsedPercentage = _elapsedTime / _timeToWaypoint;
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
            transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

            if (elapsedPercentage >= 1)
            {
                TargetNextWaypoint();
            }

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

}


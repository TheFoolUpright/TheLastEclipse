using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeSoulAgent : MonoBehaviour
{
    private StateMachine _stateMachine;
    public StateMachine StateMachine => _stateMachine;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent NavMeshAgent => agent;

    [Header("Progression")]
    [SerializeField] private int catchesRequired = 3;
    [SerializeField] private int currentCatchCount = 0;

    public int CatchesRequired => catchesRequired;
    public int CurrentCatchCount => currentCatchCount;

    [Header("Wander Movement")]
    [SerializeField] private float wanderSpeed = 2f;
    public float WanderSpeed => wanderSpeed;

    [SerializeField] private float wanderPointReachedDistance = 0.25f;
    public float WanderPointReachedDistance => wanderPointReachedDistance;

    [SerializeField] private float minWanderIdleTime = 0.5f;
    public float MinWanderIdleTime => minWanderIdleTime;

    [SerializeField] private float maxWanderIdleTime = 1.5f;
    public float MaxWanderIdleTime => maxWanderIdleTime;

    [Header("Wander Points")]
    [SerializeField] private List<Transform> wanderingPoints = new List<Transform>();
    public List<Transform> WanderingPoints => wanderingPoints;

    private Transform _currentWanderPoint;
    public Transform CurrentWanderPoint => _currentWanderPoint;

    [Header("Flee")]
    [SerializeField] private float fleeSpeedLevel0 = 4f;
    [SerializeField] private float fleeSpeedLevel1 = 4.75f;
    [SerializeField] private float fleeSpeedLevel2 = 5.5f;

    [SerializeField] private float fleeDisengageDelay = 3f;
    public float FleeDisengageDelay => fleeDisengageDelay;

    [Header("Burst")]
    [SerializeField] private float burstSpeedBonus = 2f;
    [SerializeField] private float burstDuration = 0.8f;
    public float BurstDuration => burstDuration;
    [SerializeField] private float burstTriggerDistance = 2f;
    public float BurstTriggerDistance => burstTriggerDistance;

    [Header("Stunned")]
    [SerializeField] private float stunnedDuration = 10f;
    public float StunnedDuration => stunnedDuration;

    [Header("Detection")]
    [SerializeField] private bool playerInDetectionRange;
    public bool PlayerInDetectionRange => playerInDetectionRange;

    [Header("Player")]
    [SerializeField] private Transform player;
    public Transform Player => player;

    public event Action<Collider> OnTriggerExitEvent;
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action OnCaughtByPlayer;

    public MeshRenderer fleeRenderer;

    [Header("Audio")]
    [SerializeField] private string detectedPlayerSFX = "Flee";


    private void Awake()
    {
        InitializeStateMachine();
    }

    private void Start()
    {
        _stateMachine.SwitchToNewState(typeof(STATE_FleeWander));
    }

    public void PlayDetectedPlayerSound()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySFX(detectedPlayerSFX);
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            { typeof(STATE_FleeWander), new STATE_FleeWander(this) },
            { typeof(STATE_Flee), new STATE_Flee(this) },
            { typeof(STATE_Stunned), new STATE_Stunned(this) },
            { typeof(STATE_FleeCollectable), new STATE_FleeCollectable(this) },
        };

        _stateMachine = GetComponent<StateMachine>();
        if (_stateMachine == null)
            _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.SetStates(states);
    }

    public void IncrementCatchCount()
    {
        currentCatchCount++;
    }

    public void ResetCatchCount()
    {
        currentCatchCount = 0;
    }

    public bool HasMetCatchRequirement()
    {
        return currentCatchCount >= catchesRequired;
    }

    public void SetPlayerInDetectionRange(bool value)
    {
        playerInDetectionRange = value;
    }

    public void SetWanderMovement()
    {
        agent.speed = wanderSpeed;
        agent.stoppingDistance = wanderPointReachedDistance;
    }

    public void SetFleeMovement()
    {
        agent.speed = GetCurrentBaseFleeSpeed();
        agent.stoppingDistance = 0.1f;
    }

    public void SetBurstMovement()
    {
        agent.speed = GetCurrentBaseFleeSpeed() + burstSpeedBonus;
    }

    public float GetCurrentBaseFleeSpeed()
    {
        switch (currentCatchCount)
        {
            case 0:
                return fleeSpeedLevel0;
            case 1:
                return fleeSpeedLevel1;
            default:
                return fleeSpeedLevel2;
        }
    }

    public bool ShouldTriggerBurst()
    {
        if (player == null)
            return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= burstTriggerDistance;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(targetPosition);
    }
    public void StopMoving()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
    }

    public void ClearMovement()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    public Transform ChooseRandomWanderPoint()
    {
        if (wanderingPoints == null || wanderingPoints.Count == 0)
            return null;

        if (wanderingPoints.Count == 1)
        {
            _currentWanderPoint = wanderingPoints[0];
            return _currentWanderPoint;
        }

        Transform nextPoint = _currentWanderPoint;

        // Prevent immediately picking the same point again
        while (nextPoint == _currentWanderPoint)
        {
            int randomIndex = UnityEngine.Random.Range(0, wanderingPoints.Count);
            nextPoint = wanderingPoints[randomIndex];
        }

        _currentWanderPoint = nextPoint;
        return _currentWanderPoint;
    }

    public bool HasReachedCurrentDestination()
    {
        if (!agent.isOnNavMesh)
            return false;

        if (agent.pathPending)
            return false;

        if (agent.remainingDistance == Mathf.Infinity)
            return false;

        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            return false;

        return agent.remainingDistance <= agent.stoppingDistance &&
               agent.velocity.sqrMagnitude < 0.01f;
    }

    public Transform ChooseFleePoint()
    {
        if (wanderingPoints == null || wanderingPoints.Count == 0)
            return null;

        if (player == null)
            return ChooseRandomWanderPoint();

        Vector3 awayFromPlayer = (transform.position - player.position).normalized;

        Transform bestPoint = null;
        float bestScore = float.MinValue;

        foreach (Transform point in wanderingPoints)
        {
            if (point == null)
                continue;

            if (point == _currentWanderPoint && wanderingPoints.Count > 1)
                continue;

            Vector3 toPoint = (point.position - transform.position).normalized;

            // 1 means same direction, -1 means opposite direction
            float directionScore = Vector3.Dot(awayFromPlayer, toPoint);

            float pathAwayScore = GetPathFirstStepAwayScore(point.position);
            
            // Skip points that strongly move toward the player
            if (pathAwayScore < -0.1f)
                continue;

            float distanceFromPlayer = Vector3.Distance(point.position, player.position);

            // Weighted score: prioritize direction first, distance second
            float score =
                 directionScore * 10f +
                 pathAwayScore * 20f +
                 distanceFromPlayer; 

            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = point;
            }
        }

        // fallback if all points were bad
        if (bestPoint == null)
        {
            foreach (Transform point in wanderingPoints)
            {
                if (point == null)
                    continue;

                float distanceFromPlayer = Vector3.Distance(point.position, player.position);

                if (distanceFromPlayer > bestScore)
                {
                    bestScore = distanceFromPlayer;
                    bestPoint = point;
                }
            }
        }

        if (bestPoint != null)
        {
            _currentWanderPoint = bestPoint;
        }

        return bestPoint;
    }

    public bool IsCurrentDestinationUnsafe()
    {
        if (player == null)
            return false;

        if (!agent.hasPath)
            return false;

        Vector3 awayFromPlayer = (transform.position - player.position).normalized;
        Vector3 towardDestination = (agent.destination - transform.position).normalized;

        float directionScore = Vector3.Dot(awayFromPlayer, towardDestination);

        return directionScore < 0f;
    }

    private float GetPathFirstStepAwayScore(Vector3 targetPosition)
    {
        if (player == null || !agent.isOnNavMesh)
            return 0f;

        NavMeshPath path = new NavMeshPath();

        if (!agent.CalculatePath(targetPosition, path))
            return -1f;

        if (path.status != NavMeshPathStatus.PathComplete)
            return -1f;

        if (path.corners.Length < 2)
            return -1f;

        Vector3 awayFromPlayer = transform.position - player.position;
        awayFromPlayer.y = 0f;
        awayFromPlayer.Normalize();

        Vector3 firstStep = path.corners[1] - path.corners[0];
        firstStep.y = 0f;
        firstStep.Normalize();

        return Vector3.Dot(awayFromPlayer, firstStep);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (wanderingPoints == null)
            return;

        foreach (var point in wanderingPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.3f);
        }

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, burstTriggerDistance);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);

        if (other.CompareTag("Player"))
        {
            OnCaughtByPlayer?.Invoke();
        }
    }

    public void SetStateColor(Color color)
    {
        fleeRenderer.material.color = color;
    }

}

//public enum AIState
//{
//    WANDER,
//    FLEE,
//    COLLECTABLE
//}
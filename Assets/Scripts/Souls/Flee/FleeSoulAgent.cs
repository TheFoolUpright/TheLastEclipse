using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class FleeSoulAgent : MonoBehaviour
{
    private enum MovementMode
    {
        Flying,
        NavMesh
    }

    private MovementMode _movementMode;

    private StateMachine _stateMachine;
    public StateMachine StateMachine => _stateMachine;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    public GameObject fleeSoulModel;
    public GameObject calmSoulModel;
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

    private bool _reachedDestination;

    [Header("Flee Route Nodes")]
    [SerializeField] private List<FleeNode> fleeNodes = new List<FleeNode>();

    [Header("Island Wander Nodes")]
    [SerializeField] private List<FleeNode> islandWanderNodes = new List<FleeNode>();
    public List<FleeNode> IslandWanderNodes => islandWanderNodes;

    private FleeNode _currentNode;
    private Vector3 _currentDestination;
    private bool _hasDestination;

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

    [SerializeField] private float maxDistanceFromFleeNode = 12f;

    private float _currentSpeed;

    private void Awake()
    {
        InitializeStateMachine();
    }

    private void Start()
    {
        ActiveSoul(true);
        _stateMachine.SwitchToNewState(typeof(STATE_FleeWander));

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.onPlayerRespawn += HandlePlayerRespawn;
            }
        }
    }

    private void HandlePlayerRespawn()
    {
        ResetCatchCount();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.onPlayerRespawn -= HandlePlayerRespawn;
            }
        }
    }
    public void ActiveSoul(bool isActive)
    {
        if (fleeSoulModel != null)
            fleeSoulModel.SetActive(isActive);

        if (calmSoulModel != null)
            calmSoulModel.SetActive(!isActive);
    }

    public bool IsTooFarFromFleeGraph()
    {
        if (_currentNode == null)
            return true;

        return Vector3.Distance(transform.position, _currentNode.transform.position) > maxDistanceFromFleeNode;
    }
    public FleeNode ChooseNearestFleeNode()
    {
        if (fleeNodes == null || fleeNodes.Count == 0)
            return null;

        FleeNode nearestNode = null;
        float nearestDistance = float.MaxValue;

        foreach (FleeNode node in fleeNodes)
        {
            if (node == null || !node.usableForFlee)
                continue;

            float distance = Vector3.Distance(transform.position, node.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNode = node;
            }
        }

        if (nearestNode != null)
            _currentNode = nearestNode;

        return nearestNode;
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
        _movementMode = MovementMode.NavMesh;

        if (agent != null && !agent.enabled)
            agent.enabled = true;

        TrySnapToNearestNavMesh(5f);

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = wanderSpeed;
            agent.stoppingDistance = wanderPointReachedDistance;
            agent.isStopped = false;
        }

        _hasDestination = false;
        _reachedDestination = false;
    }

    public void SetFleeMovement()
    {
        _movementMode = MovementMode.Flying;

        if (agent != null)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        _currentSpeed = GetCurrentBaseFleeSpeed();
    }

    public void SetBurstMovement()
    {
        _movementMode = MovementMode.Flying;
        _currentSpeed = GetCurrentBaseFleeSpeed() + burstSpeedBonus;
    }
    private float GetActiveSpeed()
    {
        return _currentSpeed;
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

    public bool HasActiveFlyingDestination()
    {
        return _movementMode == MovementMode.Flying && _hasDestination;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Debug.Log($"Moving to {targetPosition} in mode {_movementMode}");
        if (_movementMode == MovementMode.NavMesh)
        {
            if (agent == null || !agent.isOnNavMesh)
                return;

            _reachedDestination = false;
            agent.isStopped = false;
            agent.SetDestination(targetPosition);
            return;
        }

        _currentDestination = targetPosition;
        _hasDestination = true;
        _reachedDestination = false;
    }

    public void StopMoving()
    {
        if (_movementMode == MovementMode.NavMesh)
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;

            return;
        }

        _hasDestination = false;
    }

    public void ClearMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        _hasDestination = false;
        _reachedDestination = false;
    }

    private void Update()
    {
        if (_movementMode == MovementMode.NavMesh)
            return;

        if (!_hasDestination)
            return;

        Vector3 direction = _currentDestination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                8f * Time.deltaTime
            );
        }

        float speed = GetActiveSpeed();

        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentDestination,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _currentDestination) <= 0.15f)
        {
            _hasDestination = false;
            _reachedDestination = true;
        }
    }

    public FleeNode ChooseRandomWanderPoint()
    {
        if (islandWanderNodes == null || islandWanderNodes.Count == 0)
            return null;

        if (islandWanderNodes.Count == 1)
        {
            _currentNode = islandWanderNodes[0];
            return _currentNode;
        }

        FleeNode nextNode = _currentNode;

        while (nextNode == _currentNode)
        {
            int randomIndex = UnityEngine.Random.Range(0, islandWanderNodes.Count);
            nextNode = islandWanderNodes[randomIndex];
        }

        _currentNode = nextNode;
        return _currentNode;
    }
    public FleeNode ChooseRandomConnectedNode()
    {
        if (_currentNode == null || _currentNode.connectedNodes == null || _currentNode.connectedNodes.Count == 0)
            return ChooseRandomFleeNode();

        FleeNode nextNode = _currentNode;

        while (nextNode == _currentNode && _currentNode.connectedNodes.Count > 1)
        {
            int randomIndex = UnityEngine.Random.Range(0, _currentNode.connectedNodes.Count);
            nextNode = _currentNode.connectedNodes[randomIndex];
        }

        _currentNode = nextNode;
        return _currentNode;
    }

    public FleeNode ChooseRandomFleeNode()
    {
        if (fleeNodes == null || fleeNodes.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, fleeNodes.Count);
        _currentNode = fleeNodes[randomIndex];
        return _currentNode;
    }

    public bool HasReachedCurrentDestination()
    {
        if (_movementMode == MovementMode.NavMesh)
        {
            if (agent == null || !agent.isOnNavMesh)
                return false;

            if (agent.pathPending)
                return false;

            if (agent.remainingDistance == Mathf.Infinity)
                return false;

            return agent.remainingDistance <= agent.stoppingDistance &&
                   agent.velocity.sqrMagnitude < 0.01f;
        }

        if (!_reachedDestination)
            return false;

        _reachedDestination = false;
        return true;
    }

    public FleeNode ChooseFleePoint()
    {
        
        if (_currentNode == null || !_currentNode.usableForFlee || _currentNode.connectedNodes == null || _currentNode.connectedNodes.Count == 0)
            return ChooseBestStartingFleeNode();

        if (player == null)
            return ChooseRandomConnectedNode();

        FleeNode bestNode = null;
        float bestScore = float.MinValue;

        foreach (FleeNode node in _currentNode.connectedNodes)
        {
            if (node == null || !node.usableForFlee || node == _currentNode)
                continue;

            Vector3 awayFromPlayer = transform.position - player.position;
            awayFromPlayer.y = 0f;
            awayFromPlayer.Normalize();

            Vector3 toNode = node.transform.position - transform.position;
            toNode.y = 0f;
            toNode.Normalize();

            float directionScore = Vector3.Dot(awayFromPlayer, toNode);
            float distanceFromPlayer = Vector3.Distance(node.transform.position, player.position);

            float score = directionScore * 10f + distanceFromPlayer;

            if (score > bestScore)
            {
                bestScore = score;
                bestNode = node;
            }
        }

        if (bestNode != null)
            _currentNode = bestNode;

        return bestNode;
    }

    private FleeNode ChooseBestStartingFleeNode()
    {
        if (fleeNodes == null || fleeNodes.Count == 0)
            return null;

        FleeNode bestNode = null;
        float bestScore = float.MinValue;

        foreach (FleeNode node in fleeNodes)
        {
            if (node == null || !node.usableForFlee)
                continue;

            float distanceToSoul = Vector3.Distance(transform.position, node.transform.position);
            float distanceFromPlayer = player != null
                ? Vector3.Distance(player.position, node.transform.position)
                : 0f;

            // Prefer nearby flee nodes that are also away from the player.
            float score = distanceFromPlayer - distanceToSoul;

            if (score > bestScore)
            {
                bestScore = score;
                bestNode = node;
            }
        }

        if (bestNode != null)
            _currentNode = bestNode;

        return bestNode;
    }

    public bool TrySnapToNearestNavMesh(float sampleRadius = 3f)
    {
        if (agent == null)
            return false;

        if (!agent.enabled)
            agent.enabled = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            return true;
        }

        Debug.LogWarning("Could not find nearby NavMesh point for flee soul.");
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (islandWanderNodes != null)
        {
            foreach (var point in islandWanderNodes)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.transform.position, 0.3f);
            }
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
    public bool IsCurrentDestinationUnsafe()
    {
        if (player == null || !_hasDestination)
            return false;

        Vector3 awayFromPlayer = transform.position - player.position;
        awayFromPlayer.y = 0f;
        awayFromPlayer.Normalize();

        Vector3 towardDestination = _currentDestination - transform.position;
        towardDestination.y = 0f;
        towardDestination.Normalize();

        float directionScore = Vector3.Dot(awayFromPlayer, towardDestination);

        return directionScore < -0.25f;
    }
}

//public enum AIState
//{
//    WANDER,
//    FLEE,
//    COLLECTABLE
//}
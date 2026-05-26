using UnityEngine;

public class AttackSoulPlayerOutArenaDetection : MonoBehaviour
{
    private StateMachine _stateMachine;
    [SerializeField] public AttackSoulAgent soulAgent;

    private void Start()
    {
        _stateMachine = soulAgent.StateMachine;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out var controller))
            {
                Debug.Log("Player left area");
                if (_stateMachine.CurrentState is STATE_AttackPreperation)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackIdle));
                }
                else if (_stateMachine.CurrentState is STATE_Attack)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackIdle));
                }
            }
        }
    }
}

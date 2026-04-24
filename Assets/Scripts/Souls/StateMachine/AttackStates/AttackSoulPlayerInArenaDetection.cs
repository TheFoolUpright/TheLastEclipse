using UnityEngine;

public class AttackSoulPlayerInArenaDetection : MonoBehaviour
{

    private StateMachine _stateMachine;
    [SerializeField] public AttackSoulAgent soulAgent;

    private void Start()
    {
        _stateMachine = soulAgent.StateMachine;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                if (_stateMachine.CurrentState is STATE_AttackIdle)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackPreperation));
                }
            }
        }
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

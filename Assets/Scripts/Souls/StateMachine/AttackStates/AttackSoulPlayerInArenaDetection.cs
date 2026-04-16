using UnityEngine;

public class AttackSoulPlayerInArenaDetection : MonoBehaviour
{
    [HideInInspector] public PlayerController player;

    private StateMachine _stateMachine;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                player = controller;
                if (_stateMachine.CurrentState is STATE_AttackIdle)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackWander));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                player = controller;
                if (_stateMachine.CurrentState is STATE_AttackWander)
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

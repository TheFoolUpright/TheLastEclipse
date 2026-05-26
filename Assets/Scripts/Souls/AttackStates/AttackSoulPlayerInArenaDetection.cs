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
        if (other.gameObject)
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            if (controller)
            {
                if (_stateMachine.CurrentState is STATE_AttackIdle)
                {
                    _stateMachine.SwitchToNewState(typeof(STATE_AttackPreperation));
                }
            }
        }
    }
}

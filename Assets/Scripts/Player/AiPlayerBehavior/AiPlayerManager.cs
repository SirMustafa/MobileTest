using UnityEngine;

public class AiPlayerManager : MonoBehaviour
{
    AiPlayerBase currentState;
    AiPlayerMovement MovementState = new AiPlayerMovement();

    private void Start()
    {
        currentState = MovementState;
        currentState.OnEnterState(this);
    }
    private void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
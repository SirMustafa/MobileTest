using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float Speed;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _controller;
    private Vector2 _input;
    private float _verticalVelocity;
    private float currentSpeed;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        currentSpeed = Speed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        RotateTowardsInput(_input);
    }

    private void LateUpdate()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -1f;

        _verticalVelocity += gravity * Time.deltaTime;
        Vector3 move = new Vector3(_input.x, 0f, _input.y) * currentSpeed;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void RotateTowardsInput(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f) return;

        float targetY = 0f;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            targetY = input.x > 0 ? 90f : -90f;
        }
        else
        {
            targetY = input.y > 0 ? 0f : 180f;
        }

        transform.rotation = Quaternion.Euler(0f, targetY, 0f);
    }
}
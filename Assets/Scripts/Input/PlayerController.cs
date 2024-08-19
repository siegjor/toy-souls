using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables: Movement
    private CharacterController _characterController;
    private Vector2 _input;
    private Vector3 _direction;
    [SerializeField] private Movement movement;
    #endregion

    #region Variables: Rotation
    [SerializeField] private float rotationSpeed = 500f;
    #endregion

    #region Variables: Gravity
    private readonly float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;
    #endregion

    #region Variables: Jump
    [SerializeField] private float jumpPower;
    #endregion

    private Camera _mainCamera;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    void Update()
    {
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        _direction = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(_input.x, 0.0f, _input.y);
        Quaternion targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }

    private void ApplyMovement()
    {
        float targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);
        _characterController.Move(movement.currentSpeed * Time.deltaTime * _direction);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0.0f, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_characterController.isGrounded) return;

        _velocity += jumpPower;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        movement.isSprinting = context.started || context.performed;
    }
}

[Serializable]
public struct Movement
{
    public float speed;
    public float multiplier;
    public float acceleration;

    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}
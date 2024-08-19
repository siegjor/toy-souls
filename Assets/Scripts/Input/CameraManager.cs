using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform target;
    private float _distanceToPlayer;
    private Vector2 _input;

    [SerializeField] private MouseSensitivity mouseSensitivity;
    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation cameraRotation;

    #endregion

    private void Awake()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    public void Look(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        cameraRotation.Yaw += _input.x * mouseSensitivity.horizontal * BoolToInt(mouseSensitivity.invertHorizontal) * Time.deltaTime;
        cameraRotation.Pitch += _input.y * mouseSensitivity.vertical * BoolToInt(mouseSensitivity.invertVertical) * Time.deltaTime;
        cameraRotation.Pitch = Mathf.Clamp(cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(cameraRotation.Pitch, cameraRotation.Yaw, 0.0f);
        transform.position = target.position - transform.forward * _distanceToPlayer;
    }

    private static int BoolToInt(bool b) => b ? 1 : -1;
}

[Serializable]
public struct MouseSensitivity
{
    public float horizontal;
    public float vertical;
    public bool invertHorizontal;
    public bool invertVertical;
}

public struct CameraRotation
{
    public float Pitch;
    public float Yaw;
}

[Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLookWithMouse : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public InputActionAsset InputAction;
    public Transform HorizontalRotationObject;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = 0, mouseY = 0;

        var delta = InputAction.FindAction("FirstPlayer/ViewAround").ReadValue<Vector2>();

        mouseX = delta.x;
        mouseY = delta.y;

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        if (-mouseY > 25) 
        {
            mouseY = 0;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        HorizontalRotationObject.Rotate(Vector3.up * mouseX);
    }
}

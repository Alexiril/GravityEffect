using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class cameraPlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 5f;
    public float fastSpeed = 10f;
    public float jumpHeight = 2f;
    public AnimationCurve LSC;
    public float SlipCoefficient = 0.5f;
    public InputActionAsset InputAction;
    public bool CanMove = true;

    float FallingAccFramesTimer = 50f;
    Vector3 velocity;
    bool isGrounded;
    float x_a;
    float z_a;
    float faft = 0;

    void Start()
    {
    }

    void Update()
    {
        float x;
        float z;
        bool jumpPressed = false;
        bool acsPressed = false;

        //isGrounded = controller.isGrounded;

        var delta = InputAction.FindAction("FirstPlayer/Move").ReadValue<Vector2>();
        x = delta.x;
        z = delta.y;
        jumpPressed = Mathf.Approximately(InputAction.FindAction("FirstPlayer/Jump").ReadValue<float>(), 1);
        acsPressed = Mathf.Approximately(InputAction.FindAction("FirstPlayer/Run").ReadValue<float>(), 1);

        x_a = (isGrounded && faft == 0) ? x : x_a;
        z_a = (isGrounded && faft == 0) ? z : z_a;

        Vector3 move = transform.right * x_a + transform.forward * z_a;
        if (CanMove)
            controller.Move(move * ((isGrounded && faft > 0) ? speed*LSC.Evaluate(faft / FallingAccFramesTimer) : (acsPressed ? fastSpeed : speed)) * Time.deltaTime);

        if(jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            faft = 1;
        }

        if (velocity.y > 0 || !isGrounded) velocity.y += Physics.gravity.y * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (faft >= FallingAccFramesTimer) faft=0;
        if (faft > 0 && isGrounded) faft+=1;
    }

    // void OnControllerColliderHit(ControllerColliderHit hit)
    // {
    //     Collider cld = hit.collider;

    //     if (hit.moveDirection.y < -0.3)
    //     {
    //         FallingAccFramesTimer = (int)Math.Round(cld.material.dynamicFriction * (1.0f / Time.deltaTime) * SlipCoefficient,0);
    //     }
    // }

}

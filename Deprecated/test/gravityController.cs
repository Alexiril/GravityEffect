using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class gravityController : MonoBehaviour
{
    public GameObject DefaultGravityPlane;
    public GameObject CurrentGravityPlane;
    public Transform PersonRoot;
    public Transform PlayerRotationObject;

    public float GravityStrength = -9.81f;
    public float RotatingToGravitySpeed;
    public float RotatingApproximationCoefficient;

    public InputActionAsset InputAction;
    public int ChangingGravityDelay = 100;

    private GameObject PreviousGravityPlane;

    // Should be true if delay after previous changing was done
    private bool CanChangeGravity = true;
    private int ChangingGravityDelayCounter = 0;

    private bool NeedToStandUp = false;
    private Vector3 PlayerRotationObjectForward;

    private bool ApproximatelyEqualVector3(Vector3 a, Vector3 b, float difference) {
        if ((a-b).magnitude < difference) { return true; }
        else { return false; }
    }

    public void ChangeGravity(GameObject GravityPlane) {
        if (CurrentGravityPlane != GravityPlane) {
            PlayerRotationObjectForward = PlayerRotationObject.forward;
            PreviousGravityPlane = CurrentGravityPlane;
            CurrentGravityPlane = GravityPlane;
            Physics.gravity = CurrentGravityPlane.transform.TransformDirection(CurrentGravityPlane.transform.GetChild(0).localPosition) * GravityStrength;
            NeedToStandUp = true;
        }
    }

    void Start() {
        ChangeGravity(DefaultGravityPlane);
    }

    Vector3 ae;

    void Update() {
        if (ChangingGravityDelayCounter == ChangingGravityDelay) { CanChangeGravity = true; }
        if (CanChangeGravity) {
            bool cgPressed = Mathf.Approximately(InputAction.FindAction("FirstPlayer/ChangeGravity").ReadValue<float>(), 1);
            RaycastHit hit;
            if (cgPressed) {
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    ChangeGravity(hit.collider.gameObject);
                    ChangingGravityDelayCounter = 0;
                    CanChangeGravity = false;
                }
            }
        }

        //if (ae.magnitude != 0) Debug.DrawRay(PersonRoot.position, ae * 20, Color.yellow);
        Debug.DrawRay(PersonRoot.position, PersonRoot.forward * 20, Color.red);
        Debug.DrawRay(PersonRoot.position, PersonRoot.up * 20, Color.green);
        Debug.DrawRay(PlayerRotationObject.position, PlayerRotationObject.forward * 20, Color.white);

        if (Mathf.Approximately(InputAction.FindAction("FirstPlayer/StandUp").ReadValue<float>(), 1) && NeedToStandUp) {
            PersonRoot.up = CurrentGravityPlane.transform.TransformDirection(CurrentGravityPlane.transform.GetChild(0).localPosition);
            PlayerRotationObject.forward = PlayerRotationObjectForward;
            //ae = PersonRoot.forward;
            //PersonRoot.forward = PreviousGravityPlane.transform.TransformDirection(PreviousGravityPlane.transform.GetChild(0).localPosition);
            NeedToStandUp = false;
        }

        // Debug.Log(PlayerRotationObject.localRotation);
        // if (NeedToRecountGravity) {
        //     Vector3 topRotation = transform.localRotation.eulerAngles;
        //     Vector3 topDirection = CurrentGravityPlane.transform.TransformDirection(CurrentGravityPlane.transform.GetChild(0).localPosition);
        //     if (!ApproximatelyEqualVector3(transform.up, topDirection, RotatingApproximationCoefficient)) {
        //         transform.up += topDirection * Time.deltaTime * RotatingToGravitySpeed; 
        //     }
        //     else { 
        //         NeedToRecountGravity = false;
        //         //transform.up = topDirection;
        //     }
        //     //transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, 0f, transform.localRotation.eulerAngles.z);
        // }
    }

    void FixedUpdate() {
        if (!CanChangeGravity) { 
            ChangingGravityDelayCounter += 1;
        }
    }
}

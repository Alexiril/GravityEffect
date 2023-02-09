using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class unusualGravityRigidbody : MonoBehaviour
{

    // Private variables
    [SerializeField]
    private float velocityThreshold = 0.0001f;
    [SerializeField]
    private bool floatToSleep = false;
    [SerializeField]
    private float submergenceOffset = 0.5f;
    [SerializeField, Min(0.1f)]
    private float submergenceRange = 1f;
    [SerializeField, Min(0f)]
    private float buoyancy = 1f;
    [SerializeField, Range(0f, 10f)]
    private float waterDrag = 1f;
    [SerializeField]
    private LayerMask waterMask = 0;
    [SerializeField]
    Vector3 buoyancyOffset = Vector3.zero;

    private Rigidbody body;
    private Renderer render;
    private float floatDelay, submergence;
    private Vector3 gravity;

    // Private functions
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        render = GetComponent<Renderer>();
    }
    private void FixedUpdate()
    {
        if (floatToSleep)
        {
            if (body.IsSleeping())
            {
                floatDelay = 0f;
                return;
            }
            if (body.velocity.sqrMagnitude < velocityThreshold)
            {
                floatDelay += Time.deltaTime;
                if (floatDelay >= 1f)
                {
                    return;
                }
            }
            else
            {
                floatDelay = 0f;
            }
        }
        gravity = unusualGravity.GetGravity(body.position);
        if (submergence > 0f)
        {
            float drag =
                Mathf.Max(0f, 1f - waterDrag * submergence * Time.deltaTime);
            body.velocity *= drag;
            body.angularVelocity *= drag;
            body.AddForceAtPosition(
                gravity * -(buoyancy * submergence),
                transform.TransformPoint(buoyancyOffset),
                ForceMode.Acceleration
            );
            submergence = 0f;
        }
        body.AddForce(gravity, ForceMode.Acceleration);
        if (render.isVisible)
        {
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            body.interpolation = RigidbodyInterpolation.None;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!body.IsSleeping() && (waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence();
        }
    }
    private void EvaluateSubmergence()
    {
        Vector3 upAxis = -gravity.normalized;
        if (Physics.Raycast(
            body.position + upAxis * submergenceOffset,
            -upAxis, out RaycastHit hit, submergenceRange + 1f,
            waterMask, QueryTriggerInteraction.Collide
        ))
        {
            submergence = 1f - hit.distance / submergenceRange;
        }
        else
        {
            submergence = 1f;
        }
    }
}
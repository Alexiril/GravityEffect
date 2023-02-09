using UnityEngine;
using UnityEngine.Events;

public class automaticInterpolator : MonoBehaviour
{

    // Public classes/structs
    [System.Serializable]
    public class OnValueChangedEvent : UnityEvent<float> { }
    [System.Serializable]
    public class PositionInterpolatorSettings
    {
        public bool enable = false;
        public Rigidbody body = default;
        public Vector3 from = default, to = default;
        public Transform relativeTo = default;
    }
    [System.Serializable]
    public class PositionInterpolator
    {

        // Public variables
        public PositionInterpolatorSettings pis;

        // Public functions
        public void Interpolate(float t)
        {
            Vector3 p;
            if (pis.relativeTo)
            {
                p = Vector3.LerpUnclamped(
                    pis.relativeTo.TransformPoint(pis.from), pis.relativeTo.TransformPoint(pis.to), t
                );
            }
            else
            {
                p = Vector3.LerpUnclamped(pis.from, pis.to, t);
            }
            pis.body.MovePosition(p);
        }
    }

    // Public variables
    public bool Reversed = false;
    public bool autoReverse = false;

    // Private variables
    [SerializeField, Min(0.01f)]
    private float duration = 1f;
    [SerializeField]
    private bool smoothstep = false;
    [SerializeField]
    private PositionInterpolatorSettings positionInterpolatorSettings = new PositionInterpolatorSettings();
    [SerializeField]
    private OnValueChangedEvent onValueChanged = default;

    private float value;
    private bool positionInterpolatorEnabled;
    private float SmoothedValue => 3f * value * value - 2f * value * value * value;
    private PositionInterpolator positionInterpolator = new PositionInterpolator();

    // Private functions
    private void Awake()
    {
        positionInterpolatorEnabled = positionInterpolatorSettings.enable;
        positionInterpolator.pis = positionInterpolatorSettings;
    }
    private void FixedUpdate()
    {
        float delta = Time.deltaTime / duration;
        if (Reversed)
        {
            value -= delta;
            if (value <= 0f)
            {
                if (autoReverse)
                {
                    value = Mathf.Min(1f, -value);
                    Reversed = false;
                }
                else
                {
                    value = 0f;
                    enabled = false;
                }
            }
        }
        else
        {
            value += delta;
            if (value >= 1f)
            {
                if (autoReverse)
                {
                    value = Mathf.Max(0f, 2f - value);
                    Reversed = true;
                }
                else
                {
                    value = 1f;
                    enabled = false;
                }
            }
        }
        if (positionInterpolatorEnabled)
        {
            positionInterpolator.Interpolate(smoothstep ? SmoothedValue : value);
        }
        onValueChanged.Invoke(smoothstep ? SmoothedValue : value);
    }
}
using UnityEngine;

public class unusualGravitySource : MonoBehaviour
{

    // Public functions
    public virtual Vector3 GetGravity(Vector3 position)
    {
        return Physics.gravity;
    }

    // Private functions
    private void OnEnable()
    {
        unusualGravity.Register(this);
    }
    private void OnDisable()
    {
        unusualGravity.Unregister(this);
    }
}
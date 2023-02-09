using UnityEngine;

public class gravityGunStandard : MonoBehaviour
{
    // Consts
    const string levelTag = "Level";

    // Private variables
    [SerializeField]
    private LayerMask gravityObjectMask;
    [SerializeField, Min(0f)]
    private float gravityChangeDistance;
    [SerializeField, Min(0f)]
    private float gravityChangeDelay;

    private float lastGravityChange;

    // Public functions
    public void ChangeGravitySource()
    {
        if (Time.unscaledTime - lastGravityChange < gravityChangeDelay)
        {
            return;
        }
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, gravityChangeDistance, gravityObjectMask))
        {
            lastGravityChange = Time.unscaledTime;
            ChangeToGravityZone(hit.transform.GetChild(0).gameObject);
            if (hit.transform.childCount > 1)
            {
                for (int i = 0; i < hit.transform.childCount; i++)
                {
                    EnableGravityZone(hit.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    // Private functions
    private static void EnableGravityZone(GameObject gravityObject)
    {
        GameObject.FindWithTag(levelTag).GetComponent<level>().EnableGravityZone(gravityObject);
    }
    private static void DisableGravityZone(GameObject gravityObject)
    {
        GameObject.FindWithTag(levelTag).GetComponent<level>().DisableGravityZone(gravityObject);
    }
    private static void ChangeToGravityZone(GameObject gravityObject)
    {
        GameObject.FindWithTag(levelTag).GetComponent<level>().ChangeToGravityZone(gravityObject);
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class level : MonoBehaviour
{

    // Public classes
    [System.Serializable]
    public class standardLevel
    {
        // Public variables
        public bool enableStandardLevel;
        public bool turnOffGravityAtStart = true;
        public string gravityObjectsTag;

        // Private variables
        private List<GameObject> currentGravityObjects = new List<GameObject>();

        // Public functions
        public void EnableGravityZone(GameObject gravityObject)
        {
            if (enableStandardLevel) gravityObject.SetActive(true);
        }
        public void EnableGravityZone(List<GameObject> gravityObject)
        {
            if (enableStandardLevel)
            {
                foreach (GameObject obj in gravityObject)
                {
                    EnableGravityZone(obj);
                }
            }
        }
        public void DisableGravityZone(GameObject gravityObject)
        {
            if (enableStandardLevel) gravityObject.SetActive(false);
        }
        public void DisableGravityZone(List<GameObject> gravityObject)
        {
            if (enableStandardLevel)
            {
                foreach (GameObject obj in gravityObject)
                {
                    DisableGravityZone(obj);
                }
            }
        }
        public void ChangeToGravityZone(GameObject gravityObject)
        {
            if (enableStandardLevel)
            {
                DisableGravityZone(currentGravityObjects);
                currentGravityObjects.Clear();
                currentGravityObjects.Add(gravityObject);
                EnableGravityZone(currentGravityObjects);
            }
        }
        public void TurnOffGravityAtAll()
        {
            if (enableStandardLevel)
            {
                DisableGravityZone(currentGravityObjects);
                currentGravityObjects.Clear();
            }
        }
    }
    [System.Serializable]
    public class GravityZoneStateEvent : UnityEvent<GameObject> { }

    // Private variables
    [SerializeField]
    public string loader, nextLevel;
    [SerializeField]
    private bool LockCursor;
    [SerializeField]
    private GravityZoneStateEvent enableGravityZone = default, disableGravityZone = default, changeToGravityZone = default;
    [SerializeField]
    private standardLevel StandardLevel;

    // Public functions
    public void EnableGravityZone(GameObject gravityObject)
    {
        StandardLevel.EnableGravityZone(gravityObject);
        enableGravityZone.Invoke(gravityObject);
    }
    public void DisableGravityZone(GameObject gravityObject)
    {
        StandardLevel.DisableGravityZone(gravityObject);
        disableGravityZone.Invoke(gravityObject);
    }
    public void ChangeToGravityZone(GameObject gravityObject)
    {
        StandardLevel.ChangeToGravityZone(gravityObject);
        changeToGravityZone.Invoke(gravityObject);
    }

    // Private functions
    private void Awake()
    {
        if (LockCursor) Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        if (StandardLevel.enableStandardLevel && StandardLevel.turnOffGravityAtStart)
        {
            foreach (GameObject gravityObject in GameObject.FindGameObjectsWithTag(StandardLevel.gravityObjectsTag))
            {
                gravityObject.SetActive(false);
            }
        }
        LoadLoader();
    }
    private void LoadLoader()
    {
        SceneManager.LoadScene(loader, LoadSceneMode.Additive);
    }
}
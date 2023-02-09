using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class loader : MonoBehaviour
{

    // Private variables
    [SerializeField]
    private string levelTag;
    [SerializeField]
    private UnityEvent beforeChangeScene = default, afterChangeScene = default, beforeDestroyLoader = default, afterDestroyLoader = default;

    private string nextSceneNameToLoad;
    private string theSceneNameToUnload;

    // Public functions
    public void ChangeScene()
    {
        beforeChangeScene.Invoke();
        level thisLevel = GameObject.FindWithTag(levelTag).GetComponent<level>();
        nextSceneNameToLoad = thisLevel.nextLevel;
        theSceneNameToUnload = thisLevel.gameObject.scene.name;
        StartCoroutine(UnloadLevel(theSceneNameToUnload));
        StartCoroutine(LoadLevel(nextSceneNameToLoad));
        afterChangeScene.Invoke();
    }
    public void DestroyLoader()
    {
        beforeDestroyLoader.Invoke();
        SceneManager.UnloadSceneAsync(gameObject.scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        afterDestroyLoader.Invoke();
    }
    private IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        yield return SceneManager.LoadSceneAsync(
            levelBuildIndex, LoadSceneMode.Additive
        );
        SceneManager.SetActiveScene(
            SceneManager.GetSceneByBuildIndex(levelBuildIndex)
        );
        enabled = true;
    }
    private IEnumerator LoadLevel(string levelName)
    {
        enabled = false;
        yield return SceneManager.LoadSceneAsync(
            levelName, LoadSceneMode.Additive
        );
        SceneManager.SetActiveScene(
            SceneManager.GetSceneByName(levelName)
        );
        enabled = true;
    }
    private IEnumerator UnloadLevel(int levelBuildIndex, int levelToMakeActive = -1)
    {
        enabled = false;
        if (levelToMakeActive == -1) levelToMakeActive = gameObject.scene.buildIndex;
        SceneManager.SetActiveScene(
            SceneManager.GetSceneByBuildIndex(levelToMakeActive)
        );
        yield return SceneManager.UnloadSceneAsync(
            levelBuildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects
        );
        enabled = true;
    }
    private IEnumerator UnloadLevel(string levelName, string levelToMakeActive = "")
    {
        enabled = false;
        if (levelToMakeActive == "") levelToMakeActive = gameObject.scene.name;
        SceneManager.SetActiveScene(
            SceneManager.GetSceneByName(levelToMakeActive)
        );
        yield return SceneManager.UnloadSceneAsync(
            levelName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects
        );
        enabled = true;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public const string bootstrapSceneName = "Bootstrapper";
    public const string titleScreenSceneName = "TitleScreen";
    public const string combatScreenSceneName = "Combat";
    public const string treeScreenSceneName = "UpgradeTree";

    private static bool isInitialized;

    private void Awake()
    {
        if (isInitialized)
        {
            Destroy(gameObject);
            return;
        }

        isInitialized = true;
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;

        SaveSystem.Initialize();
    }

    private void Start()
    {
        if (SceneManager.loadedSceneCount == 1)
        {
            SceneManager.LoadScene(titleScreenSceneName, LoadSceneMode.Additive);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
#if UNITY_EDITOR
        Scene currentlyLoadedEditorScene = SceneManager.GetActiveScene();
#endif

        if (!SceneManager.GetSceneByName(bootstrapSceneName).isLoaded)
        {
            SceneManager.LoadScene(bootstrapSceneName, LoadSceneMode.Single);
        }

#if UNITY_EDITOR
        if (currentlyLoadedEditorScene.IsValid())
        {
            SceneManager.LoadScene(currentlyLoadedEditorScene.name, LoadSceneMode.Additive);
        }
#else
        SceneManager.LoadScene(titleScreenSceneName, LoadSceneMode.Additive);
#endif
    }
}

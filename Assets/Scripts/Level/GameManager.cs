using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private LevelManager activeLevelManager;
    private LevelData activeLevelData;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetActiveLevelManager(LevelManager levelManager)
    {
        activeLevelManager = levelManager;
        if (activeLevelManager != null) Debug.Log("Added level manager" + activeLevelManager.name);
    }

    public void SetActiveLevelData(LevelData levelData)
    {
        activeLevelData = levelData;
        if(activeLevelData != null) Debug.Log("Changed level to " + activeLevelData.levelName);
    }

    public void StartLevel()
    {
        if (activeLevelData == null)
        {
            Debug.LogError("No level set! Cant start");
            return;
        }
        Debug.Log("Started level " + activeLevelData.levelName);
        SceneManager.LoadSceneAsync(activeLevelData.sceneName);
    }
}

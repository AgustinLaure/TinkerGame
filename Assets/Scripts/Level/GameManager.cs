using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    private LevelManager activeLevelManager;
    private LevelData activeLevelData;

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetActiveLevelManager(LevelManager levelManager)
    {
        activeLevelManager = levelManager;
        Debug.Log("Added level manager" + activeLevelManager.name);
    }

    public void SetActiveLevelData(LevelData levelData)
    {
        activeLevelData = levelData;
        Debug.Log("Changed level to " + activeLevelData.levelName);
    }

    public void StartLevel()
    {
        Debug.Log("Started level " + activeLevelData.levelName);
        SceneManager.LoadScene(activeLevelData.sceneName);
    }
}

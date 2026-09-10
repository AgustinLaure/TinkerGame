using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private LevelManager activeLevel;

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetActiveLevel(LevelManager level)
    {
        activeLevel = level;
        Debug.Log("Changed level to " + activeLevel.name);
    }

    public void StartLevel()
    {
        Debug.Log("Started level " + activeLevel.name);
    }
}

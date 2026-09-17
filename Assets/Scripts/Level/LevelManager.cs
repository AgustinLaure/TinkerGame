using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;

    private EventBus eventBus;

    [SerializeField] private GameObject playerSpawnPoint;

    private bool isPaused = false;

    public bool IsPaused { get { return isPaused; } } 


    void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        gameManager = ServiceLocator.Instance.GetService<GameManager>();
        gameManager.SetActiveLevelManager(this);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        if (gameManager != null) gameManager.SetActiveLevelManager(null);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 1.0f : 0.0f ;
        eventBus.Raise<OnPause>(IsPaused);
        Debug.Log("Game is " + (IsPaused? "Unp" : "P") + "aused");
    }

    public void OnWin()
    {
        eventBus.Raise<OnWin>();
    }
    public void OnLose()
    {
        eventBus.Raise<OnLose>();
    }
}

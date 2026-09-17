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
        if (gameManager != null) gameManager.SetActiveLevelManager(null);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log("Pause state: " + IsPaused);
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

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;

    private EventBus eventBus;

    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    public bool IsPaused { get { return isPaused; } } 


    void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        gameManager = ServiceLocator.Instance.GetService<GameManager>();
        gameManager.SetActiveLevelManager(this);

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        if (gameManager != null) gameManager.SetActiveLevelManager(null);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0f : 1.0f ;
        eventBus.Raise<OnPause>(IsPaused);
        if (pausePanel != null) pausePanel.SetActive(IsPaused);
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

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;


    [SerializeField] private GameObject playerSpawnPoint; 
    
    void Awake()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>();
        gameManager.SetActiveLevelManager(this);
    }

    private void OnDestroy()
    {
        if (gameManager != null) gameManager.SetActiveLevelManager(null);
    }

    public void OnWin()
    {

    }
    public void OnLose()
    {

    }
}

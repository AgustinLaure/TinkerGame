using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameManager gameManager;
    void Awake()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>();
        gameManager.SetActiveLevelManager(this);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

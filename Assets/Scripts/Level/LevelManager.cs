using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;

    private EventBus eventBus;

    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private Transform defaultCamPos;
     public Transform DefaultCamPos { get { return defaultCamPos; }}

    [SerializeField] private Camera cam;

    [SerializeField] private int currentLayerIndex = 0;

    [SerializeField] private List<LayerController> layers = new List<LayerController>();

    private bool isPaused = false;

    public bool IsPaused { get { return isPaused; } } 

    void Awake()
    {
        if (layers.Count > 0)
        {
            Debug.LogWarning("Dont fill the layers manually!");
            layers.Clear();
        }

        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        gameManager = ServiceLocator.Instance.GetService<GameManager>();
        gameManager.SetActiveLevelManager(this);

        if (gameManager.levelManager == null)
        {
            Debug.LogWarning("Level should be started from gameManager!");
            gameManager.SetActiveLevelManager(this);
            Debug.LogWarning("Level data will be empty.");
        }

        if (pausePanel != null) pausePanel.SetActive(false);
    }
    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();

        layers = layers.OrderBy(g => g.transform.position.z).ToList();

        for (int i = 0; i < layers.Count; i++) 
        {
            layers[i].SetLayerIndex(i);
            layers[i].RecalculateIndexes();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
        if (gameManager != null) gameManager.SetActiveLevelManager(null);
    }

    public void RegisterLayer(LayerController layer)
    {
        layers.Add(layer);
    }

    public void SetCurrentLayer(int layerIndex)
    {
        currentLayerIndex = layerIndex;
        Debug.Log("camera from: " + cam.transform.position);
        cam.transform.position = layers[currentLayerIndex].camPos.position;
        Debug.Log("camera to : " + cam.transform.position);
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

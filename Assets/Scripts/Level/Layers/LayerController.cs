using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] public Transform camPos;

    [SerializeField] public GameObject terrainObject;
    [SerializeField] public GameObject foregroundObject;
    [SerializeField] public SpriteRenderer fadeLayerSprite;

    [SerializeField] public bool overrideFadeColor;
    [SerializeField] public Color fadeColor;

    [SerializeField] private List<LayerPortal> portalList = new List<LayerPortal>();
    [SerializeField] private List<PortalTarget> portalTargets = new List<PortalTarget>();

    private LevelManager levelManager;

    private int layerIndex = -1;

    public int LayerIndex { get { return layerIndex; } }
    public void SetLayerIndex(int newIndex) { layerIndex = newIndex; }

    void Start()
    {
        levelManager = ServiceLocator.Instance.GetService<GameManager>().levelManager;
        levelManager.RegisterLayer(this);

        if (!camPos)
        {
            GameObject obj = new GameObject();

            obj.name = "DefaultCamPos";
            obj.transform.SetParent(this.transform);
            obj.transform.SetPositionAndRotation(levelManager.DefaultCamPos.position + this.transform.position, levelManager.DefaultCamPos.rotation);

            camPos = obj.transform;
        }

        if (!terrainObject)
        {
            if (transform.Find("Terrain")) terrainObject = transform.Find("Terrain").gameObject;
            if (!terrainObject)
            {
                Debug.LogError("No terrain has been found for this layer!");
            }
        }

        if (!foregroundObject)
        {
            if (transform.Find("Foreground")) foregroundObject = transform.Find("Foreground").gameObject;
        }

        if(!overrideFadeColor)
        {
            fadeColor = levelManager.defaultFadeColor;
            fadeLayerSprite.color = fadeColor;
        }
    }

    public void RecalculateIndexes()
    {
        foreach (LayerPortal portal in portalList)
        {
            portal.SetLayerIndex(layerIndex);
        }
        foreach (PortalTarget target in portalTargets)
        {
            target.SetLayerIndex(layerIndex);
        }
    }

    public void SetVisible(bool visible)
    {
        StartCoroutine(FadeCorroutine(visible));
    }

    IEnumerator FadeCorroutine(bool visible)
    {
        float alpha = 0.0f;

        float progress = 0.0f;

        Debug.Log("FadeColor alpha " + fadeColor.a);
        
        while (progress < 1.0f)
        {
            alpha = Mathf.Lerp(0.0f,fadeColor.a, progress);
            progress += Time.deltaTime;
            Debug.Log("Alpha " + alpha + " progress: " + progress);

            fadeLayerSprite.color = new Color(fadeLayerSprite.color.r, fadeLayerSprite.color.g, fadeLayerSprite.color.b, (visible ? alpha : (1.0f - alpha)));
        }
        

        yield return null;
    }

    void Update()
    {
        
    }
}

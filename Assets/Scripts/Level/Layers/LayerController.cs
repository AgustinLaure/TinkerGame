using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] public Transform camPos;

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

    void Update()
    {
        
    }
}

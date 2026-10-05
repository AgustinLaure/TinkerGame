using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] Transform camPos;

    private int layerIndex = -1;

    public int LayerIndex { get { return layerIndex; } }
    public void SetLayerIndex(int newIndex) { layerIndex = newIndex; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

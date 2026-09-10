using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public abstract class LevelBase : MonoBehaviour
{
    [SerializeField] public List<GameObject> layers = new List<GameObject>();

    private void Update()
    {
        int layersAmount = layers.Count;

        if (layersAmount == 0) return;

        layers[0].transform.position = Vector3.zero;

        if (layersAmount <= 1) return;

        for (int i = 1; i < layersAmount - 1; i++)
        {
            Vector3 currentPos = Vector3.zero;
            currentPos.z = layers[layersAmount - 1].transform.position.z * (float)((float)i / ((float)layersAmount -1));
            layers[i].transform.position = currentPos;
        }
    }
}

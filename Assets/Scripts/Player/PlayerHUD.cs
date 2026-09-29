using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform symbolPoint;
    [SerializeField] private RectTransform pivotTransform;
    [SerializeField] private Sprite[] symbolSprites;
    [SerializeField] private Image[] symbols;
    private EventBus eventBus;

    [Header("Config")]
    [SerializeField] private float distanceBetweenSymbols;

    private Camera mainCamera;

    private void Start()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();
        mainCamera = Camera.main;

        eventBus.Subscribe<OnPlayerUpdateCraftMoves>((Action<OnPlayerUpdateCraftMoves>)HandlePlayerUpdateCraftMoves);
    }

    private void LateUpdate()
    {
        Vector3 symbolPointScreen = mainCamera.WorldToScreenPoint(symbolPoint.position);

        pivotTransform.position = symbolPointScreen;
    }

    private void HandlePlayerUpdateCraftMoves(OnPlayerUpdateCraftMoves data)
    {
        PlayerCraft.Move[] moves = data.moves;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (moves[i] != PlayerCraft.Move.None)
            {
                symbols[i].gameObject.SetActive(true);
                symbols[i].sprite = symbolSprites[(int)moves[i]];
            }
            else
            {
                symbols[i].gameObject.SetActive(false);
            }
        }
    }
}

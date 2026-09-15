using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerCraft : MonoBehaviour
{
    private enum Move
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    private const int maxRecipes = 1;

    [Header("References")]
    [SerializeField] private OrigamiPool origamiPool;
    [SerializeField] private Transform origamiSpawnPointTRS;
    private EventBus eventBus;

    [Header("Config")]
    [SerializeField] private const int maxPossibleMoves = 10;
    [SerializeField] private Move[] aicraftRecipe;

    private struct Recipe
    {
        public Move[] moves;
        public Type type;
    }

    private Recipe[] recipes = new Recipe[maxRecipes];

    private Move[] cachedMoves = new Move[maxPossibleMoves];
    private int cachedMovesPointer = 0;

    private bool isCrafting = false;
    bool isKeyDown = false;


    private void Awake()
    {
        eventBus = ServiceLocator.Instance.GetService<EventBus>();

        recipes[0].moves = aicraftRecipe;
        recipes[0].type = typeof(Aircraft);
    }

    private void Start()
    {

    }

    private void OnToggleCraft(InputValue value)
    {
        if (enabled)
        {
            isCrafting = !isCrafting;

            eventBus.Raise<OnPlayerToggleCraft>(isCrafting);

            if (isCrafting)
            {
                ResetCache();
            }
        }
    }

    private void ResetCache()
    {
        for (int i = 0; i < maxPossibleMoves; i++)
        {
            cachedMoves[i] = Move.None;
        }
        cachedMovesPointer = 0;
    }

    private void OnCraftMoves(InputValue value)
    {
        if (isCrafting)
        {
            Vector2 axis = value.Get<Vector2>();

            if (!isKeyDown)
            {
                Move move = Move.None;

                if (axis.x > 0f)
                {
                    move = Move.Right;
                }
                else if (axis.x < 0f)
                {
                    move = Move.Left;
                }
                else if (axis.y > 0f)
                {
                    move = Move.Up;
                }
                else if (axis.y < 0f)
                {
                    move = Move.Down;
                }

                isKeyDown = true;

                AddMove(move);
            }

            if (axis == Vector2.zero)
            {
                isKeyDown = false;
            }
        }
    }

    private void OnDrop(InputValue value)
    {
        if (isCrafting)
        {
            ResetCache();
        }
    }

    private bool AddMove(Move move)
    {
        if (cachedMovesPointer >= maxPossibleMoves)
        {
            return false;
        }

        cachedMoves[cachedMovesPointer] = move;
        cachedMovesPointer++;

        for (int i = 0; i < maxRecipes; i++)
        {
            if (MatchesRecipe(recipes[i].moves, cachedMoves))
            {
                GameObject origami = origamiPool.GetOrigami(recipes[i].type, origamiSpawnPointTRS.position, Quaternion.identity, transform);

                eventBus.Raise<OnPlayerCraftedOrigami>(origami.GetComponent<Origami>());

                isCrafting = false;
                isKeyDown = false;

                break;
            }
        }

        return true;
    }

    private bool MatchesRecipe(Move[] recipe, Move[] moves)
    {
        int movesLength = 0;

        foreach (Move move in moves)
        {
            if (move != Move.None)
            {
                movesLength++;
            }
        }

        if (recipe.Length != movesLength)
        {
            return false;
        }

        bool matches = true;

        for (int i = 0; i < recipe.Length; i++)
        {
            if (recipe[i] != moves[i])
            {
                matches = false;
            }
        }

        return matches;
    }
}

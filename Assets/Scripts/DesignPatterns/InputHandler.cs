using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
}

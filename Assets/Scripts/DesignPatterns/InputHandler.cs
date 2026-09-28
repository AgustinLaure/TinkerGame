using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput GetPlayerInput { get; private set; }

    private void Awake()
    {
        GetPlayerInput = GetComponent<PlayerInput>();
    }
}

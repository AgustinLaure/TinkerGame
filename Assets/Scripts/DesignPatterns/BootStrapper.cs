using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public static class BootStrapper
{
    private const string inputHandlerRoute = "InputHandler";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Init()
    {
        GameObject inputHandlerPrefab = Resources.Load<GameObject>(inputHandlerRoute);

        GameObject inputHandlerGO = Object.Instantiate(inputHandlerPrefab, Vector3.zero, Quaternion.identity);

        ServiceLocator serviceLocator = ServiceLocator.Instance;

        serviceLocator.AddService(inputHandlerGO.GetComponent<PlayerInput>());
        serviceLocator.AddService(new CompositePool());
        serviceLocator.AddService(new EventBus());
        serviceLocator.AddService(new GameManager());
    }
}

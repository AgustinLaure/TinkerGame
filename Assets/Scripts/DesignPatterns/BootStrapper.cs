using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public static class BootStrapper
{
    private const string inputHandlerRoute = "InputHandler";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Init()
    {
        GameObject inputHandlerGO = Addressables.InstantiateAsync(inputHandlerRoute,Vector3.zero,Quaternion.identity).WaitForCompletion();
        
        ServiceLocator serviceLocator = ServiceLocator.Instance;

        serviceLocator.AddService(inputHandlerGO.GetComponent<PlayerInput>());
        serviceLocator.AddService(new CompositePool());
        serviceLocator.AddService(new EventBus());
        serviceLocator.AddService(new GameManager());
    }
}

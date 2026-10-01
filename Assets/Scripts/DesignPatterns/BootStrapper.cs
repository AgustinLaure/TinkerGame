using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public static class BootStrapper
{
    private static ServiceLocator serviceLocator = ServiceLocator.Instance;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        serviceLocator.AddServiceAsGameObject<InputHandler>(null);

        serviceLocator.AddService(new CompositePool());
        serviceLocator.AddService(new EventBus());
        serviceLocator.AddService(new GameManager());
    }
}

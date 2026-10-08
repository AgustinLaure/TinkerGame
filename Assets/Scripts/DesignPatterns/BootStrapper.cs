using UnityEngine;
using UnityEngine.Audio;

public static class BootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Init()
    {
        ServiceLocator.Instance.AddService(new CompositePool());
        ServiceLocator.Instance.AddService(new EventBus());
    }
}

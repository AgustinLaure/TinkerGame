using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestInput : MonoSingleton<TestInput>
{
    private GameManager gameManager;

    public LevelData levelData1;
    public LevelData levelData2;

    private int option = 0;

    private bool onMain = true;

    void Start()
    {
        gameManager = ServiceLocator.Instance.GetService<GameManager>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !onMain)
        {
            SceneManager.LoadScene("MainMenuTest");
            onMain = true;
        }
        if (Keyboard.current.numpad1Key.isPressed && option != 1)
        {
            gameManager.SetActiveLevelData(levelData1);
            option = 1;
        }
        if (Keyboard.current.numpad2Key.isPressed && option != 2)
        {
            gameManager.SetActiveLevelData(levelData2);
            option = 2;
        }
        if (Keyboard.current.spaceKey.isPressed && onMain)
        {
            gameManager.StartLevel();
            option = 3;
            onMain = false;
        }
    }
}

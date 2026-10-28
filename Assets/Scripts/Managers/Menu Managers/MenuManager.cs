using Unity.VisualScripting;
using UnityEngine;

public enum MenuType
{
    MAIN,
    ENDSCREEN
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject endScreenPanel;

    private void Awake()
    {
        GameStateManager.Instance.OnLoadMenu += OnLoadMenu;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnLoadMenu -= OnLoadMenu;
    }

    private void OnLoadMenu(MenuType menu)
    {
        switch(menu)
        {
            case MenuType.MAIN:
                SwitchToMainMenu();
                break;
            case MenuType.ENDSCREEN:
                SwitchToEndScreen();
                break;
        }
    }

    public void PlayGame()
    {
        GameStateManager.Instance.RequestStartGame();
    }

    public void ReturnToMenu()
    {
        SwitchToMainMenu();
    }

    private void SwitchToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        endScreenPanel.SetActive(false);
    }

    private void SwitchToEndScreen()
    {
        endScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}

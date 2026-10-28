using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { LOADING, MENU, INGAME };

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public GameState GameState { get; private set; }
    private bool avaliable;

    public Action<MenuType> OnLoadMenu;
    public Action OnStartGame;

    private GameStateManager() => Instance = this;

    private void Start()
    {
        GameState = GameState.LOADING;
        avaliable = true;
        RequestMenu(MenuType.MAIN);
    }

    public void RequestMenu(MenuType menu)
    {
        if (avaliable && (GameState == GameState.LOADING || GameState == GameState.INGAME))
            StartCoroutine(LoadMenu(menu));
    }

    private IEnumerator LoadMenu(MenuType menu)
    {
        avaliable = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while(!asyncLoad.isDone) yield return null;

        avaliable = true;
        SetGameState(GameState.MENU);
        OnLoadMenu?.Invoke(menu);
    }

    public void RequestStartGame()
    {
        if(avaliable && GameState == GameState.MENU)
            StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        avaliable = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        while (!asyncLoad.isDone) yield return null;

        avaliable = true;
        SetGameState(GameState.INGAME);
        OnStartGame?.Invoke();
    }

    private void SetGameState(GameState state)
    {
        if (state == GameState) return;
        GameState = state;
        Debug.Log("Switched to " + state);
    }
}

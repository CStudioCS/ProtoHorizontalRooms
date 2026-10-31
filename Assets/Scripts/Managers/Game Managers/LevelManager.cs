using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private Player player1Prefab;
    [SerializeField] private Player player2Prefab;
    public Player Player1Instance { get; private set; }
    public Player Player2Instance { get; private set; }

    [SerializeField] private MapDefinition Map;
    private Room roomInstance;
    private int currentRoomIndex;
    public bool IsTransitioning { get; private set; }

    public Action OnRoomEnter;
    public Action OnTransitionBegin;

    [SerializeField] private TransitionEffectManager transitionEffectManager;

    private LevelManager() => Instance = this;

    private void Awake()
    {
        GameStateManager.Instance.OnStartGame += OnStartGame;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnStartGame -= OnStartGame;
    }

    private void OnStartGame()
    {
        Player1Instance = Instantiate(player1Prefab);
        Player2Instance = Instantiate(player2Prefab);

        currentRoomIndex = Map.StartingRoomIndex;
        BeginTransition(Map.Rooms[currentRoomIndex], Vector3.zero);
    }

    public void ChangeRoom(bool right)
    {
        if (GameStateManager.Instance.GameState != GameState.INGAME) return;

        int delta = (right ? 1 : -1);
        currentRoomIndex += delta;
        if(currentRoomIndex < 0 || currentRoomIndex >= Map.Rooms.Length)
        {
            Debug.Log(currentRoomIndex - delta + "->" + currentRoomIndex);
            GameStateManager.Instance.RequestMenu(MenuType.ENDSCREEN);
            return;
        }

        Room nextRoom = Map.Rooms[currentRoomIndex];


        // GOAL : entered + roomPos = exit + nextRoomPos
        Vector3 enteredDoorLocalPosition, exitDoorLocalPosition;
        if(right)
        {
            enteredDoorLocalPosition = roomInstance.GetRightDoorPosition();
            exitDoorLocalPosition = nextRoom.GetLeftDoorPosition();
        }
        else
        {
            enteredDoorLocalPosition = roomInstance.GetLeftDoorPosition();
            exitDoorLocalPosition = nextRoom.GetRightDoorPosition();
        }

        Vector3 nextRoomPos = roomInstance.transform.position + enteredDoorLocalPosition - exitDoorLocalPosition;

        Debug.Log(nextRoomPos);

        BeginTransition(nextRoom, nextRoomPos);
    }

    private void BeginTransition(Room nextRoomPrefab, Vector3 nextRoomPos)
    {
        if(!IsTransitioning && nextRoomPrefab != null)
            StartCoroutine(TransitionRoom(nextRoomPrefab, nextRoomPos));
    }

    private IEnumerator TransitionRoom(Room nextRoomPrefab, Vector3 nextRoomPos)
    {
        IsTransitioning = true;
        OnTransitionBegin?.Invoke();

        Room oldRoomInstance = roomInstance;
        Room nextRoomInstance = Instantiate(
            nextRoomPrefab,
            nextRoomPos,
            Quaternion.identity);

        if(transitionEffectManager) yield return transitionEffectManager.TransitionEffect(nextRoomPos);

        roomInstance = nextRoomInstance;

        if(oldRoomInstance != null) Destroy(oldRoomInstance.gameObject);

        IsTransitioning = false;
        OnRoomEnter?.Invoke();
    }
}

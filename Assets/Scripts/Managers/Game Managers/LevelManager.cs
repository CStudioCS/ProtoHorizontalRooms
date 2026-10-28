using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private Player player1Prefab;
    [SerializeField] private Player player2Prefab;
    public Player Player1Instance { get; private set; }
    public Player Player2Instance { get; private set; }

    [SerializeField] private MapDefinition Map;
    private RoomDefinition Room;
    private GameObject roomInstance;
    public bool isTransitioning { get; private set; }

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

        BeginTransition(Map.StartingRoom);
    }

    public void ChangeRoom(bool right)
    {
        if (GameStateManager.Instance.GameState != GameState.INGAME) return;

        Vector3 roomPosition = Map.GetRoomPosition(Room);
        var roomBounds = Room.GetBounds(roomPosition);
        RoomDefinition nextRoom = Map.GetRoomAt(new Vector3(
            (right ? roomBounds.xMax + 0.01f : roomBounds.xMin - 0.01f),
            roomPosition.y,
            roomPosition.z
        ));

        if(nextRoom == null)
        {
            GameStateManager.Instance.RequestMenu(MenuType.ENDSCREEN);
            return;
        }

        BeginTransition(nextRoom);
    }

    private void BeginTransition(RoomDefinition nextRoom)
    {
        if(!isTransitioning && nextRoom != null)
            StartCoroutine(TransitionRoom(nextRoom));
    }

    private IEnumerator TransitionRoom(RoomDefinition nextRoom)
    {
        isTransitioning = true;
        OnTransitionBegin?.Invoke();

        Vector3 nextRoomPos = Map.GetRoomPosition(nextRoom);

        GameObject oldRoomInstance = roomInstance;
        GameObject nextRoomInstance = Instantiate(
            nextRoom.roomPrefab,
            nextRoomPos,
            Quaternion.identity);

        if(transitionEffectManager) yield return transitionEffectManager.TransitionEffect(nextRoomPos);

        Room = nextRoom;
        roomInstance = nextRoomInstance;

        if(oldRoomInstance != null) Destroy(oldRoomInstance);

        isTransitioning = false;
        OnRoomEnter?.Invoke();
    }
}

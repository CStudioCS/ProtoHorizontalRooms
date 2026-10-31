using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private PlayerTeleporter player1TeleportPoint;
    [SerializeField] private PlayerTeleporter player2TeleportPoint;

    [SerializeField] private RoomTransitionDetector leftRoomDoor;
    [SerializeField] private RoomTransitionDetector rightRoomDoor;

    [SerializeField] private RoomBounds roomSizeDelimitor;

    private void OnEnable()
    {
        LevelManager.Instance.OnRoomEnter += OnRoomEnter;
    }
    private void OnDisable()
    {
        LevelManager.Instance.OnRoomEnter -= OnRoomEnter;
    }

    public Vector2 GetRoomSize()
    {
        return roomSizeDelimitor.transform.localScale;
    }

    public Vector3 GetLeftDoorPosition()
    {
        return leftRoomDoor.transform.localPosition;
    }

    public Vector3 GetRightDoorPosition()
    {
        return rightRoomDoor.transform.localPosition;
    }

    private void OnRoomEnter()
    {
        TeleportPlayers();
    }

    private void TeleportPlayers()
    {
        if (player1TeleportPoint)
            LevelManager.Instance.Player1Instance.transform.position = player1TeleportPoint.transform.position;
        if (player2TeleportPoint)
            LevelManager.Instance.Player2Instance.transform.position = player2TeleportPoint.transform.position;
    }
}

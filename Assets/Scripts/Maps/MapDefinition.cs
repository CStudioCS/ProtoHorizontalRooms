using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomPosition
{
    public RoomDefinition room;
    public Vector3 position;
}

[CreateAssetMenu(fileName = "MapDefinition", menuName = "Mapping/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    public RoomPosition[] rooms;
    public RoomDefinition StartingRoom;

    public Vector3 GetRoomPosition(RoomDefinition room)
    {
        foreach(var roomPos in rooms)
        {
            if (roomPos.room == room) return roomPos.position;
        }
        throw new ArgumentException("Room not registered is map !");
    }

    public RoomDefinition GetRoomAt(Vector3 position)
    {
        foreach (var roomPos in rooms)
        {
            if (roomPos.room.GetBounds(roomPos.position).Contains(position))
                return roomPos.room;
        }

        return null;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDefinition", menuName = "Mapping/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    public Room[] Rooms;
    public int StartingRoomIndex;
}

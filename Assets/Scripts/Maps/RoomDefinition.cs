using UnityEngine;

[CreateAssetMenu(fileName = "RoomDefinition", menuName = "Mapping/RoomDefinition")]
public class RoomDefinition : ScriptableObject
{
    public string roomId;
    public GameObject roomPrefab;
    public Vector2 roomSize;

    public Rect GetBounds(Vector3 center)
    {
        return new Rect((Vector2)center - roomSize * 0.5f, roomSize);
    }
}

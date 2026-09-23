using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //This is a singleton (look it up !)
    public static GameManager Instance { get; private set; }

    [Header("Mapping")]

    public const int STARTING_MAP_INDEX = 1;
    /// <summary>
    /// Gets decreased by one when the screen moves to the left and increased by one when it moves to the right.
    /// </summary>
    public int CurrentMapIndex { get; private set; } = STARTING_MAP_INDEX;
    public Map[] Maps; //contains prefabs of maps
    public Map CurrentMap; //contains the map currently in the scene (instantiated)

    [Header("Players")]
    public Player Player1;
    public Player Player2;

    [Header("Transition Settings")]
    [SerializeField] private Camera Camera;
    [SerializeField] private ScreenTransition LeftTransition;
    [SerializeField] private ScreenTransition RightTransition;
    [SerializeField] private float TransitionDuration;
    [SerializeField] private float OffsetX;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CurrentMap.RespawnPlayer(Player1);
        CurrentMap.RespawnPlayer(Player2);
    }

    public IEnumerator TransitionRoutine(bool isLeftTransition, Player transitioningPlayer)
    {
        //deactivate transition
        LeftTransition.CanTransition = false;
        RightTransition.CanTransition = false;

        int mapIndexChange = isLeftTransition ? -1 : 1;
        Map newMap = Instantiate(Maps[CurrentMapIndex + mapIndexChange], new Vector3((CurrentMapIndex - STARTING_MAP_INDEX + mapIndexChange) * OffsetX, 0, 0), Quaternion.identity);

        //smooth transition coroutine
        float time = 0;
        Vector3 initPos = Camera.transform.position;

        while (time < TransitionDuration)
        {
            Camera.transform.position = Vector3.Lerp(initPos, initPos + new Vector3(OffsetX, 0, 0) * mapIndexChange, time / TransitionDuration);

            time += Time.deltaTime;
            yield return null;
        }

        Camera.transform.position = initPos + new Vector3(OffsetX, 0, 0) * mapIndexChange;

        //move player who triggered the transition
        Vector2 playerMoveOffset = new Vector2(1.5f, 0);
        transitioningPlayer.transform.position += mapIndexChange * (Vector3)playerMoveOffset;

        //Destroy previous map and set current map to the new one
        Destroy(CurrentMap.gameObject);
        CurrentMapIndex += mapIndexChange;
        CurrentMap = newMap;

        //respawn player 2
        CurrentMap.RespawnPlayer(transitioningPlayer.OtherPlayer);


        //Maybe here give the new items to the players ??

        LeftTransition.CanTransition = true;
        RightTransition.CanTransition = true;
    }
}

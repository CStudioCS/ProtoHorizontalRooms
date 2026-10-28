using System;
using UnityEngine;

public class PlayerTeleporter: MonoBehaviour
{
    [SerializeField] private bool teleportsPlayer1;
    [SerializeField] private bool teleportsPlayer2;

    private void OnEnable()
    {
        LevelManager.Instance.OnRoomEnter += OnRoomEnter;
    }
    private void OnDisable()
    {
        LevelManager.Instance.OnRoomEnter -= OnRoomEnter;
    }

    private void OnRoomEnter()
    {
        if(teleportsPlayer1)
            LevelManager.Instance.Player1Instance.transform.position = transform.position;
        if (teleportsPlayer2)
            LevelManager.Instance.Player2Instance.transform.position = transform.position;
    }
}

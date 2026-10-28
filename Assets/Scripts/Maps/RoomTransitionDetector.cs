using UnityEngine;

public class RoomTransitionDetector : MonoBehaviour
{
    [SerializeField] private bool isRightTransition;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameStateManager.Instance.GameState != GameState.INGAME) return;

        if (!LevelManager.Instance.isTransitioning && other.TryGetComponent<Player>(out Player player))
        {
            LevelManager.Instance.ChangeRoom(isRightTransition);
        }
    }
}

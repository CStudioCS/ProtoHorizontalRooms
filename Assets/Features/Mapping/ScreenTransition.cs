using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private bool isLeftTransition;
    [HideInInspector] public bool CanTransition = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CanTransition && other.TryGetComponent<Player>(out Player player) && ((isLeftTransition && !player.IsPlayer1) || (!isLeftTransition && player.IsPlayer1)))
        {
            StartCoroutine(GameManager.Instance.TransitionRoutine(isLeftTransition, player));
        }
    }
}

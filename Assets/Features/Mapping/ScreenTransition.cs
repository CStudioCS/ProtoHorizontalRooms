using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private bool isLeftTransition;
    [HideInInspector] public bool CanTransition = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CanTransition && other.TryGetComponent<Crate>(out Crate crate))
            StartCoroutine(GameManager.Instance.TransitionRoutine(isLeftTransition));
    }
}

using UnityEngine;

public class GravityField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetInvertedGravity(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetInvertedGravity(false);
        }
    }
}

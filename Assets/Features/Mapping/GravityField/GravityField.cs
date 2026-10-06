using UnityEngine;

public class GravityField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetInvertedGravity(true);
            if(player.GrabCrate.CrateJoint.enabled)
            {
                player.GrabCrate.TryDrop();
                player.GrabCrate.Grab();
            }
        }
        else if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.gravityScale = -1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetInvertedGravity(false);
            if(player.GrabCrate.CrateJoint.enabled)
            {
                player.GrabCrate.TryDrop();
                player.GrabCrate.Grab();
            }
        }
        else if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.gravityScale = 1;
        }
    }
}

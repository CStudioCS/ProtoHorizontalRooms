using UnityEngine;

public class Crate : MonoBehaviour
{
    public Rigidbody2D Rb;
    public BoxCollider2D Collider;
    public Player holdingPlayer;

    [SerializeField] private float heldMass = 0.2f;
    [SerializeField] private float notHeldMass = 1f;

    private void Awake()
    {
        Rb.mass = notHeldMass;
    }

    private void Update()
    {
        if (GameManager.Instance.Player1.GrabCrate.CrateJoint.enabled)
            holdingPlayer = GameManager.Instance.Player1;
        else if (GameManager.Instance.Player2.GrabCrate.CrateJoint.enabled)
            holdingPlayer = GameManager.Instance.Player2;
        else
            holdingPlayer = null;

        if (holdingPlayer != null)
            Rb.mass = heldMass;
        else Rb.mass = notHeldMass;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!(collision.gameObject.TryGetComponent(out Player player) && player == holdingPlayer))
        {
            GameManager.Instance.Player1.GrabCrate.TryDrop();
            GameManager.Instance.Player2.GrabCrate.TryDrop();
        }
    }
}


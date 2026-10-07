using UnityEngine;

public class Tremplin_script : MonoBehaviour
{
    [SerializeField] private float speed;

    private void OnTriggerStay2D(Collider2D other)
    {
        //recuperer l'objet, vérifier que c'est un joueur, obtenir le rigid
        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            rb.linearVelocity = transform.up * speed + Vector2.Dot(rb.linearVelocity, transform.right) * transform.right;

    }
}
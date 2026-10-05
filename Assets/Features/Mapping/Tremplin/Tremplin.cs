using UnityEngine;

public class Tremplin_script : MonoBehaviour
{
    [SerializeField] private float speed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //recuperer l'objet, vérifier que c'est un joueur, obtenir le rigid
        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            float rotation = (transform.eulerAngles.z) * Mathf.Deg2Rad;
            rb.linearVelocityY = Mathf.Cos(rotation) * speed;
            rb.linearVelocityX = Mathf.Sin(rotation) * speed;
        }

    }
}
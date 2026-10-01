using UnityEngine;

public class Tremplin_script : MonoBehaviour
{
    [SerializeField] private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //recuperer l'objet, vérifier que c'est un joueur, obtenir le rigid
        if (other.TryGetComponent<Player>(out Player player))
        {
            float rotation = (transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
            float x = Mathf.Cos(rotation) * speed;
            float y = Mathf.Sin(rotation) * speed;
            Debug.Log("rot" + rotation);
            player.rb.linearVelocityY = y;
            player.rb.linearVelocityX = x;

            Debug.Log("x = " + x);
            Debug.Log("y = " + y);
        }

    }
}
using UnityEngine;

public class Map : MonoBehaviour
{
    public Vector2 Player1Spawn;
    public Vector2 Player2Spawn;
    public void RespawnPlayer(Player player)
    {
        if (player.IsPlayer1)
            player.transform.position = transform.position + (Vector3)Player1Spawn;
        else
            player.transform.position = transform.position + (Vector3)Player2Spawn;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Player1Spawn, 0.2f);
        Gizmos.DrawWireSphere(Player2Spawn, 0.2f);
    }
}

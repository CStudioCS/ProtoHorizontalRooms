using JetBrains.Annotations;
using UnityEngine;

public class GrapplingHook : Ability
{

    private bool isGrappling = false;

    public override void OnUse(Player player)
    {
        if (isGrappling)
        {
            isGrappling = false;
            return;
        }

        if (player.OtherPlayer == null)
        {
            Debug.Log("No other player found!");
            return;
        }

        isGrappling = true;
    }

    public override void OnUpdate(Player player)
    {
        if (!isGrappling)
        {
            return;
        }

        Vector2 targetPosition = player.OtherPlayer.transform.position;
        Vector2 playerPosition = player.transform.position;

        Vector2 direction = targetPosition - playerPosition;

        float distance = direction.magnitude;

        if (distance <= player.stopGrappleDistance)
        {
            isGrappling = false;
            player.OtherPlayer.rb.linearVelocity = Vector2.zero;
            return;
        }

        if (distance <= player.maxGrappleDistance)
        {
            isGrappling = false;
            return;
        }

        direction.Normalize();
        player.OtherPlayer.rb.linearVelocity = direction * player.grappleSpeed;
    }
}
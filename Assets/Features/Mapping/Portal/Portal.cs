using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public Portal OtherPortal;
    public float teleportCooldown = 0.1f;

    private Dictionary<Rigidbody2D, float> teleportCooldowns = new Dictionary<Rigidbody2D, float>();

    private void Update()
    {
        List<Rigidbody2D> keysToRemove = null;

        foreach (var kvp in teleportCooldowns)
        {
            if (kvp.Key == null || Time.time >= kvp.Value)
            {
                keysToRemove ??= new List<Rigidbody2D>();
                keysToRemove.Add(kvp.Key);
            }
        }

        if (keysToRemove != null)
        {
            foreach (var key in keysToRemove)
            {
                teleportCooldowns.Remove(key);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(OtherPortal && other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))) return;

        if (other.TryGetComponent<Crate>(out Crate crate) && crate.holdingPlayer) return;

        if (teleportCooldowns.TryGetValue(rb, out float expirationTime))
        {
            if (Time.time < expirationTime) return;
        }
        OtherPortal.teleportCooldowns[rb] = Time.time + teleportCooldown;

        TeleportObject(rb);
        if (other.TryGetComponent<Player>(out Player player) && player.GrabCrate.CrateJoint.enabled)
        {
            Rigidbody2D crateRb = player.GrabCrate.CrateJoint.connectedBody;
            if (crateRb != null)
            {
                TeleportObject(crateRb);
            }
        }
    }

    private void TeleportObject(Rigidbody2D rb)
    {
        Transform inBasis = transform;
        Transform outBasis = OtherPortal.transform;

        rb.position = (Vector2)OtherPortal.transform.position + ChangeVectorBasis(rb.position - (Vector2)transform.position, inBasis, outBasis);
        rb.linearVelocity = ChangeSpeedBasis(rb.linearVelocity, inBasis, outBasis);
        if (rb.linearVelocity.sqrMagnitude < 3f * 3f) rb.linearVelocity = 3f * rb.linearVelocity.normalized;
    }

    private Vector2 ChangeVectorBasis(Vector2 vector, Transform from, Transform to)
    {
        float x = Vector2.Dot(vector, from.right);
        float y = Vector2.Dot(vector, from.up);
        return x * to.right + y * to.up;
    }

    private Vector2 ChangeSpeedBasis(Vector2 speed, Transform from, Transform to)
    {
        float x = Vector2.Dot(speed, from.right);
        float y = Vector2.Dot(speed, from.up);
        return -x * to.right + y * to.up;
    }
}
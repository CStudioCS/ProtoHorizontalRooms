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

        if (teleportCooldowns.TryGetValue(rb, out float expirationTime))
        {
            if (Time.time < expirationTime) return;
        }
        OtherPortal.teleportCooldowns[rb] = Time.time + teleportCooldown;

        rb.position = (Vector2)OtherPortal.transform.position;
        rb.linearVelocity = GetOutputSpeed(rb.linearVelocity);
    }

    private Vector2 GetOutputSpeed(Vector2 inputDirection)
    {
        // Dans la base du portail de départ
        float inputX = Vector2.Dot(inputDirection, transform.right);
        float inputY = Vector2.Dot(inputDirection, transform.up);

        // Dans la base du portail d'arrivée (Inverse en X)
        Vector2 outputDirection = -inputX * OtherPortal.transform.right +
            inputY * OtherPortal.transform.up;

        return outputDirection;
    }
}
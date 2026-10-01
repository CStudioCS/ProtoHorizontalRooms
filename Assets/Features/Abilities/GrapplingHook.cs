using JetBrains.Annotations;
using UnityEngine;

public class GrapplingHook : Ability
{

    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;
    public override void OnUpdate(Player player)
    {
        distanceJoint.enabled = false;
    }

    public override void OnUse(Player player)
    {
        Vector3 targetPos = player.OtherPlayer.transform.position;

        lineRenderer.enabled = true;
        distanceJoint.enabled = true;
        lineRenderer.SetPosition(0, targetPos);
        lineRenderer.SetPosition(1, player.transform.position);

        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(0, targetPos);
            lineRenderer.SetPosition(1, player.transform.position);
        }
    }

    public override void OnQuit(Player player)
    {
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
    }
}
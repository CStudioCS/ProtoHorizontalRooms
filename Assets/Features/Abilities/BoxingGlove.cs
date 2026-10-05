using UnityEngine;

public class BoxingGlove : Ability
{
    [SerializeField] private float forceStrength = 10f;
    public override void OnUse(Player player)
    {
        Vector2 dir = GameManager.Instance.Crate.transform.position - player.transform.position;
        dir.Normalize();

        GameManager.Instance.Crate.Rb.AddForce(dir * forceStrength, ForceMode2D.Impulse);
    }
}

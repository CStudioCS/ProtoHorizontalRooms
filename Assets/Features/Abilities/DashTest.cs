public class DashTest : Ability
{
    public override void OnUse(Player player)
    {
        player.AddSpeed(player.dashSpeed * (player.isFacingRight ? 1 : -1), 0f);
    }
}
public abstract class Ability
{
    public abstract void OnUse(Player player);
    public virtual void OnUpdate(Player player) { }
}

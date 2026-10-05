using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void OnUse(Player player);
    public virtual void OnUpdate(Player player) { }
}

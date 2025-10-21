using DarkTonic.MasterAudio;
using UnityEngine;

public class HealItem : Item
{
    [SerializeField] private float healAmount;
    public override void Use()
    {
        gm.player.health = Mathf.Min(gm.player.maxHealth, gm.player.health + healAmount);
        MasterAudio.PlaySound("Heal");
        Debug.Log("체력을 " + healAmount + "만큼 회복!");
    }
}



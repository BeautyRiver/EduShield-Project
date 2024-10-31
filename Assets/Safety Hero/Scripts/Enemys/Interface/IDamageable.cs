using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float MaxHealth { get; set; }
    public float Health { get; set; }
    public bool IsLive { get; set; }
    public void Damaged(Collider2D collision, float damage);
    public void Dead();
}
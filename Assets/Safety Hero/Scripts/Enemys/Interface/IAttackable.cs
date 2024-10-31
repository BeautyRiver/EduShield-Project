using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public float Damage { get; set; }
    public void Attack();

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float MaxHealth { get; set; }
    public float Health { get; set; }

}
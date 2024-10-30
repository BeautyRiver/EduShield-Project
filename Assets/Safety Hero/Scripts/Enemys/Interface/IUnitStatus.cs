using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStatus
{
    public float Speed { get; set; }
    public bool IsLive { get; set; }
    public int Id { get; set; }
}

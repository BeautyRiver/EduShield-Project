using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStatus
{

    public int Id { get; set; }
    void Init(SpawnData data);
}

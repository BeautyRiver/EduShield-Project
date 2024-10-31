using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropExpable
{
    public int Exp { get; set; }
    public void DropExp();
}


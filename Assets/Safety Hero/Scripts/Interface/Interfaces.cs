using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기능 관련
public interface IMovable
{
    void Move();
}
public interface IAttackable
{
    void Attack();
}
//-------------------------------------------

// 무기 관련
public interface IBatchable
{
    void Batch();
}

public interface IRotatingable
{
    void Rotate();
}

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

public interface IDamageable 
{
    void DamagedLogic(Collider2D collision, float damage);
}

public interface IRepositionable { }
public interface IKnockBackable { }


//-------------------------------------------

// 구분 관련
public interface IMiniBoss : IRepositionable, IKnockBackable { }
public interface IBoss : IRepositionable, IKnockBackable { }

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

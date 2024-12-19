using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기능 관련
// 움직일 수 있는지
public interface IMovable
{
    void Move();
}
// 공격할 수 있는지
public interface IAttackable
{
    void Attack();
}

// 부딪혔을 때 데미지 줄 수 있는지
public interface IContactDamage
{

}

// 데미지 받을 수 있는지
public interface IDamageable 
{
    void DamagedLogic(Collider2D collision, float damage);
}

// 위치 재배치할 수 있는지
public interface IRepositionable { }

// 넉백 당할 수 있는지
public interface IKnockBackable { }


//-------------------------------------------

// 구분 관련
public interface IMiniBoss : IRepositionable, IKnockBackable { }
public interface IBoss : IRepositionable, IKnockBackable { }

//-------------------------------------------


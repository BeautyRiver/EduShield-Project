using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqeEnemy : Enemy, IMovable
{
    protected override void DropReward()
    {
        GameObject expObj = gm.poolManager.Get(PoolType.Item, 0); // Exp 드랍시키기
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = this.exp;
    }

    protected override void FlipX()
    {
        // No Flip
    }

    public void Move()
    {
        rigid.MovePosition(rigid.position + (nextVec * speed * Time.fixedDeltaTime));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        nextVec = (targetRb.position - rigid.position).normalized;
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }
}
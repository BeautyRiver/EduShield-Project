using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy , IRepositionable, IKnockBackable, IMovable
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
        FlipX();
    }



    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    public void Move()
    {
        dirVec = targetRb.position - rigid.position; 
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}

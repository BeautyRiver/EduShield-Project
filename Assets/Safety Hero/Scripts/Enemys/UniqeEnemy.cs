using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqeEnemy : Enemy, IMovable
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
        FlipX();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || !coll.CompareTag("Ground"))
            return;

        gameObject.SetActive(false);
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
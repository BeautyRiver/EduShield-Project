using DarkTonic.MasterAudio;
using System.Collections;
using UnityEngine;

public class RangeEnemy : Enemy, IRepositionable, IKnockBackable, IMovable
{
    private TargetScanner scanner; // 적 탐색기        
    private bool isAttacking = false; // 현재 공격 중인지 여부

    protected override void Awake()
    {
        base.Awake();
        scanner = GetComponent<TargetScanner>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();      
    }

  
    protected override void DropReward()
    {
        GameObject expObj = GameManager.instance.poolManager.Get(PoolType.Item, 0); // expCount 생성
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = this.exp;
    }

    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    public void Move()
    {
        Vector2 dirVec = targetRb.position - rigid.position;
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}

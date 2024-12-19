using UnityEngine;

public class ExpScanner : MonoBehaviour
{
    [Header("# 경험치 획득")]
    public float expCollectionRange = 1f; // 경험치 획득 범위
    public LayerMask expLayer;

    private void FixedUpdate()
    {
        // 경험치 아이템 감지 및 획득
        CollectExp();
    }

    private void CollectExp()
    {
        Collider2D[] expItems = Physics2D.OverlapCircleAll(transform.position, expCollectionRange, expLayer);

        foreach (Collider2D item in expItems)
        {
            Exp expObj = item.GetComponent<Exp>();
            if (!expObj.IsMoving)
                expObj.ItemMoveLogic(transform);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, expCollectionRange);
    }
}

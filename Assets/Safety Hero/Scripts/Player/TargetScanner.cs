using System.Collections;
using UnityEngine;

public class TargetScanner : MonoBehaviour
{
    [Header("# 스캔 (범위 : 사거리)")]
    public float scanRange;
    [SerializeField] private Color scanColor;
    [SerializeField] private LayerMask targetLayer;

    [Header("# 옵션")]
    [Tooltip("켜면 매 스캔마다 baseScanRange * playerData.attackRangeMult로 자동 갱신.\n플레이어의 적 탐지 스캐너에만 켜고, EXP/상호작용/적 측 스캐너는 꺼둘 것.")]
    [SerializeField] private bool applyAttackRangeMultiplier = false;

    public RaycastHit2D[] targets;
    [field: SerializeField] public Transform nearestTarget { get; private set; }

    // 사거리 배율 적용 시 곱셈의 기준이 되는 값. Awake에서 인스펙터 설정값으로 캡처.
    private float baseScanRange;

    private void Awake()
    {
        baseScanRange = scanRange;
    }

    private void Start()
    {
        StartCoroutine(FindNearstTarget());
    }

    IEnumerator FindNearstTarget()
    {
        while (true)
        {
            if (GlobalManager.instance.playerState != PlayerState.FreeMove)
            {
                nearestTarget = null;
                yield return null;
                continue;
            }

            // 사거리 배율 적용 (플레이어 적 탐지 스캐너 전용)
            if (applyAttackRangeMultiplier && GameManager.instance != null && GameManager.instance.playerData != null)
            {
                scanRange = baseScanRange * GameManager.instance.playerData.attackRangeMult;
            }

            targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
            nearestTarget = GetNearest();
            yield return null;
        }
    }

    // 가장 가까운 대상 반환 함수
    public Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = scanColor;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }
}

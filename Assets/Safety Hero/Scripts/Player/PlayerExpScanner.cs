using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerExpScanner : MonoBehaviour
{
    [Header("# 경험치 획득")]
    private float originalRange;
    [SerializeField] private TargetScanner targetScanner;

    private void Awake()
    {
        originalRange = targetScanner.scanRange;
    }
    private void FixedUpdate()
    {
        // 경험치 아이템 감지 및 획득
        if (targetScanner.targets != null)
        {
            foreach (var item in targetScanner.targets)
            {
                Exp expObj = item.collider.GetComponent<Exp>();
                if (!expObj.IsMoving)
                    expObj.ItemMoveLogic(transform);
            }
        }

    }

    public void ActivateMagnet(float magentRange, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(OnMagnetEffect(magentRange, duration));
    }

    private IEnumerator OnMagnetEffect(float magentRange, float duration)
    {
        originalRange = targetScanner.scanRange;
        targetScanner.scanRange = magentRange;
        yield return new WaitForSeconds(duration);
        targetScanner.scanRange = originalRange;
    }

    //private void CollectExp()
    //{
    //    Collider2D[] expItems = Physics2D.OverlapCircleAll(transform.position, expCollectionRange, expLayer);

    //    foreach (Collider2D item in expItems)
    //    {
    //        Exp expObj = item.GetComponent<Exp>();
    //        if (!expObj.IsMoving)
    //            expObj.ItemMoveLogic(transform);
    //    }
    //}

}

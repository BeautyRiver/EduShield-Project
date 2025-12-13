using UnityEngine;
using UnityEngine.EventSystems; 
using DG.Tweening;

[RequireComponent(typeof(EventTrigger))]
public class UISelectScaler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public float scaleMultiplier = 1.1f; // 얼마나 커질지
    public float duration = 0.2f;        

    private Vector3 originalScale; // 버튼의 원래 크기를 저장할 변수

    void Awake()
    {
        originalScale = transform.localScale;
    }

    // 이 오브젝트가 '선택'되었을 때 EventSystem이 자동으로 이 함수를 호출
    public void OnSelect(BaseEventData eventData)
    {
        transform.DOKill(); // 이전에 실행 중이던 DOTween 애니메이션이 있다면 즉시 중지
        transform.DOScale(originalScale * scaleMultiplier, duration)
                 .SetEase(Ease.OutBack)
                 .SetUpdate(true); // Time.timeScale이 0이어도(일시정지) UI는 움직이도록 설정
    }

    // 이 오브젝트의 '선택'이 풀렸을 때 EventSystem이 자동으로 이 함수를 호출
    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration)
                 .SetEase(Ease.OutBack)
                 .SetUpdate(true);
    }
}
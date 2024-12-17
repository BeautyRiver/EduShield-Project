using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectorManager : MonoBehaviour
{
    public static SelectorManager instance;
    [SerializeField] private GameObject currentSelectedButton;

    // 버튼별 원래 스케일을 저장하기 위한 Dictionary
    private readonly Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // GameManager 확인 및 비활성 상태면 무시
        if (GameManager.instance != null && GameManager.instance.isGameActive)
        {
            Debug.Log("GameManager is not active");
            return;
        }

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        // 선택된 버튼이 없을 경우 마지막 선택된 버튼을 다시 선택
        if (EventSystem.current.currentSelectedGameObject == null && currentSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelectedButton);
        }

        // 선택된 버튼이 바뀌었을 때만 처리
        if (selectedObject != currentSelectedButton)
        {
            UpdateButtonScale(selectedObject, true);      // 새 버튼 확대
            UpdateButtonScale(currentSelectedButton, false); // 이전 버튼 원복

            currentSelectedButton = selectedObject; // 버튼 갱신
        }
    }

    // 버튼의 Scale 조정 (DOTween 사용)
    private void UpdateButtonScale(GameObject button, bool isSelected)
    {
        if (button == null || button.CompareTag("StageImages"))
            return;

        // 원래 스케일 저장
        if (!originalScales.ContainsKey(button))
        {
            originalScales[button] = button.transform.localScale;
        }

        Vector3 originalScale = originalScales[button];
        Vector3 targetScale = isSelected ? originalScale * 1.1f : originalScale;

        // 기존 DOTween 애니메이션을 강제로 종료
        button.transform.DOKill();

        // DOTween으로 Scale 변경
        button.transform.DOScale(targetScale, 0.2f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // TimeScale 무시
    }

}

using DG.Tweening;
using TMPro;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    [TextArea] public string[] alertMessages;
    public TextMeshProUGUI scriptText;
    public float textDuration;
    public float appearAiDuration;
    public int selectStageIdx;

    private RectTransform aiImageRect;
    private RectTransform aiTextAreaRect;

    private Vector3 originalAiImagePos;
    private Vector3 originalAiTextAreaPos;

    private void Awake()
    {
        RectTransform[] rcts = GetComponentsInChildren<RectTransform>();
        aiImageRect = rcts[1];
        aiTextAreaRect = rcts[2];

        scriptText = GetComponentInChildren<TextMeshProUGUI>();

        // 원래 위치 저장
        originalAiImagePos = aiImageRect.anchoredPosition;
        originalAiTextAreaPos = aiTextAreaRect.anchoredPosition;
    }

    public void HideAi()
    {
        MoveRectToPosition(aiImageRect, originalAiImagePos, Ease.InBack);
        MoveRectToPosition(aiTextAreaRect, originalAiTextAreaPos, Ease.InExpo);
    }

    // Ai 이미지 등장
    public void AppearAiImage(int selectStage)
    {
        MoveRectToPosition(aiImageRect, new Vector3(-3.25f, 225, 0), Ease.OutBack, () => AppearAiMessage());
    }

    // 메시지창 등장
    private void AppearAiMessage()
    {
        MoveRectToPosition(aiTextAreaRect, new Vector3(465f, 40f, 0), Ease.OutExpo, () => ShowAlertMessage());
    }

    // 메시지창 속 메시지 등장
    private void ShowAlertMessage()
    {
        scriptText.DOText(alertMessages[selectStageIdx], textDuration, true).SetEase(Ease.Linear);
    }

    // 공통된 애니메이션 동작
    private void MoveRectToPosition(RectTransform rect, Vector3 targetPos, Ease easeType, TweenCallback onComplete = null)
    {
        rect.DOAnchorPos(targetPos, appearAiDuration).SetEase(easeType).SetUpdate(true).OnComplete(onComplete);
    }
}

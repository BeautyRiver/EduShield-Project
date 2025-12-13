using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionProtector : MonoBehaviour
{
    [SerializeField] private GameObject currentSelectedButton;
    private PlayerInputController playerInputController;
    private void Start()
    {
        playerInputController = FindAnyObjectByType<PlayerInputController>();
    }
    void Update()
    {
        // UI 액션 맵이 아닐 경우 처리하지 않음        
        if (playerInputController.GetCurrentActionMap() != "UI")                    
            return;
        

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        // 선택된 버튼이 없을 경우 마지막 선택된 버튼을 다시 선택
        if (EventSystem.current.currentSelectedGameObject == null && currentSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(currentSelectedButton);
        }

        // 선택된 버튼이 바뀌었을 때만 처리
        if (selectedObject != currentSelectedButton)
        {
            currentSelectedButton = selectedObject; // 버튼 갱신
        }
    }
}

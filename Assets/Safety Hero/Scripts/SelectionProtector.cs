using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionProtector : MonoBehaviour
{
    [SerializeField] private GameObject currentSelectedButton;

    void Update()
    {
        // GameManager 확인 및 비활성 상태면 무시
        if (GameManager.instance != null && GameManager.instance.currentState == GameState.Playing)
        {            
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
            currentSelectedButton = selectedObject; // 버튼 갱신
        }
    }
}

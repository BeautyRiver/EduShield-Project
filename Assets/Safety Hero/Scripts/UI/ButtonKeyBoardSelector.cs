using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonKeyBoardSelector : MonoBehaviour
{
    [SerializeField] private Selectable titleFirstSelectButton; // 초기 선택된 버튼
    [SerializeField] private Selectable optionFirstSelectButton; // 옵션 화면에서 초기 선택된 버튼
    [SerializeField] private Selectable stageSelectFirstSelectButton; // 스테이지 선택 화면에서 초기 선택된 버튼
    [SerializeField] private GameObject currentSelectedButton;

    private void Start()
    {
        SelectTitleFirstButton();
    }

    public void SelectTitleFirstButton()
    {
        titleFirstSelectButton.Select();
    }
    public void SelectOptionFirstButton()
    {
        optionFirstSelectButton.Select();
    }
    public void SelectStageSelectFirstButton()
    {
        stageSelectFirstSelectButton.Select();
    }

    void Update()
    {        
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            currentSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
        // 현재 선택된 오브젝트가 없으면 기본 버튼을 다시 선택
        else
        {
            Debug.Log("currentSelectedButton is null");
            EventSystem.current.SetSelectedGameObject(currentSelectedButton);
        }
    }
}

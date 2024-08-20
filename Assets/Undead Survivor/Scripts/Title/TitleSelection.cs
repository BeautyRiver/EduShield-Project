using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSelection : MonoBehaviour
{
    public Transform titleButtons;
    public Button[] buttons;
    public RectTransform[] buttonRects;
    public int index;

    private void Awake()
    {
        index = 0;
        buttons = titleButtons.GetComponentsInChildren<Button>();        
        buttonRects = new RectTransform[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonRects[i] = buttons[i].gameObject.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        gameObject.transform.position = buttonRects[index].position;
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"{index}번째 버튼 실행");
            buttons[index].onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
            SetPosition();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
            SetPosition();
        }
    }
    private void MoveUp()
    {
        index--;
        if (index < 0)
            index = 0;
    }

    private void MoveDown()
    {
        index++;
        if (index >= buttons.Length)
        {
            index = buttons.Length - 1;
        }
    }

    private void SetPosition()
    {        
        gameObject.transform.SetParent(buttonRects[index].gameObject.transform);
        //gameObject.transform.position = buttonRects[index].position;
        gameObject.transform.DOMove(buttonRects[index].position, 0.15f).SetEase(Ease.OutQuart);
    }

}

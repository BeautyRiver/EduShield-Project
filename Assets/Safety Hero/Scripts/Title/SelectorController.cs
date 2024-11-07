using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EUIType
{
    TitleButtons,
    ESCButtons,
    LevelUpButtons,
}
[System.Serializable]
public class Buttons
{    
    public EUIType uiType;
    public Button[] buttons;
}
public class SelectorController : MonoBehaviour
{
    public static Action SelectorEvent;

    private Vector2 initPos;
    private RectTransform myRect;
    
    [SerializeField] private Transform itemGroup;
    [SerializeField] private ItemSetting[] itemFilter;
    [SerializeField] private RectTransform[] itemRects;
    [SerializeField] private int index; 
    
    private void Start()
    {        
        myRect = GetComponent<RectTransform>();
        initPos = myRect.anchoredPosition;
    }

    void Initialize()
    {
        itemFilter = itemGroup.GetComponentsInChildren<ItemSetting>();
        itemRects = new RectTransform[itemFilter.Length];

        for (int i = 0; i < itemFilter.Length; i++)
        {
            itemRects[i] = itemFilter[i].GetComponentInChildren<RectTransform>();
        }

        // 위치 처음 아이템으로 초기화
        index = 0;
        myRect.anchoredPosition = initPos;        
        Debug.Log("Set Pos " + itemRects[0].name);
    }

    private void Update()
    {
        if (!GameManager.instance.isGameActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"{index}번째 버튼 실행");
                var button = itemRects[index].gameObject.GetComponent<Button>();
                // 버튼을 선택 상태로 만들어서 Highlighted Color 적용
                button.Select();
                button.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log($"Right Arrow");
                IndexUp();
                SetPosition();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log($"Left Arrow");
                IndexDown();
                SetPosition();
            }
        }
    }
    private void IndexDown()
    {
        index--;
        index = Mathf.Max(index, 0);
    }

    private void IndexUp()
    {
        index++;
        index = Mathf.Min(index, itemRects.Length - 1);
    }

    private void SetPosition()
    {
        Debug.Log("Set Pos");
        myRect.DOAnchorPos(itemRects[index].anchoredPosition, 0.05f).SetEase(Ease.OutQuart).SetUpdate(true);
        //gameObject.transform.DOMove(new Vector3(0,-15.5f,0), 0.15f).SetEase(Ease.OutQuart);
    }

    private void OnEnable()
    {
        SelectorEvent += Initialize;
    }
    private void OnDisable()
    {
        SelectorEvent -= Initialize;
    }
}

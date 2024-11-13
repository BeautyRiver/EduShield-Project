using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

public class ButtonKeyBoardSelector : MonoBehaviour
{
    public static Action<GameObject> SelectorEvent;

    [SerializeField] private List<Button> selectables = new List<Button>();
    [SerializeField] private int currentIndex = 0;
    
    private Dictionary<Button, Image> buttonHighlightMap = new Dictionary<Button, Image>(); // 버튼과 하이라이트 이미지 매핑

    private Vector3 prevOriginalScale;

    private void Update()
    {
        // 게임이 일시정지되거나 UI 메뉴가 활성화된 경우에만 네비게이션을 처리합니다.
        if (!GameManager.instance.isGameActive)
        {
            HandleNavigation();
        }
    }

    public void InitializeNavigation(GameObject buttonsParents)
    {
        StartCoroutine(InitCor(buttonsParents));
    }
    
   private IEnumerator InitCor(GameObject buttonsParents)
{
    yield return null;
    selectables.Clear();
    selectables.AddRange(buttonsParents.GetComponentsInChildren<Button>());
    selectables.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

    buttonHighlightMap.Clear();
    foreach (var button in selectables)
    {
        // 버튼의 특정 자식 이미지를 미리 캐싱
        var highlightImage = button.GetComponentsInChildren<Image>(true)[1];
            if (highlightImage == null)
                break;
        buttonHighlightMap[button] = highlightImage;
    }

    if (selectables.Count > 0)
    {
        currentIndex = 0;
        SelectCurrent();
    }
}

    private void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {            
            MoveNext();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {            
            MovePrevious();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ActivateCurrent();
        }
    }

    private void MoveNext()
    {
        ResetAllButtonScales(); // 모든 버튼 크기 초기화
        currentIndex = (currentIndex + 1) % selectables.Count; // 3
        SelectCurrent();
    }

    private void MovePrevious()
    {
        ResetAllButtonScales(); // 모든 버튼 크기 초기화
        currentIndex = (currentIndex - 1 + selectables.Count) % selectables.Count; // 음수 방지
        SelectCurrent();
    }

    private void SelectCurrent()
    {
        ResetAllButtonScales(); // 모든 버튼 크기 초기화
        if (buttonHighlightMap.TryGetValue(selectables[currentIndex], out var highlightImage))
        {
            highlightImage.gameObject.SetActive(true);
        }
        selectables[currentIndex].transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack).SetUpdate(true); // 선택된 버튼 크기 증가

        var currentSelectable = selectables[currentIndex]; // 현재 선택된 버튼
        EventSystem.current.SetSelectedGameObject(currentSelectable.gameObject); // 버튼 Select 상태                                                                                         
    }

    private void ActivateCurrent()
    {
        var currentSelectable = selectables[currentIndex];
        if (currentSelectable != null)
        {
            currentSelectable.onClick.Invoke();            
        }
        // Toggle, Slider 등 다른 UI 요소에 대한 처리도 추가할 수 있습니다.
    }

    private void ResetAllButtonScales()
    {
        foreach (var button in selectables)
        {
            button.transform.localScale = Vector3.one; // DOTween 대신 직접 스케일 설정
            if (buttonHighlightMap.TryGetValue(button, out var highlightImage))
            {
                highlightImage.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        SelectorEvent += InitializeNavigation;
    }
    private void OnDisable()
    {
        SelectorEvent -= InitializeNavigation;
    }
}

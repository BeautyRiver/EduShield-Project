using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

public class ButtonKeyBoardSelector : MonoBehaviour
{
    [SerializeField] private List<Button> selectables = new List<Button>();
    [SerializeField] private int currentIndex = 0;


    private void Update()
    {
        // 게임이 일시정지되거나 UI 메뉴가 활성화된 경우에만 네비게이션을 처리합니다.
        if (!GameManager.instance.isGameActive)
        {
            HandleNavigation();
        }
    }

    public void InitializeNavigation()
    {
        selectables.Clear();

            selectables.AddRange(FindObjectsOfType<Button>());
     
        // 네비게이션 순서 정렬 (옵션)
        selectables.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

        // 첫 번째 버튼을 선택합니다.
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
        currentIndex++;
        currentIndex = Mathf.Min(selectables.Count - 1, currentIndex);
        SelectCurrent();
    }

    private void MovePrevious()
    {
        currentIndex--;
        currentIndex = Mathf.Max(0, currentIndex);
        SelectCurrent();
    }


    private void SelectCurrent()
    {
        var currentSelectable = selectables[currentIndex];

        // 현재 선택된 게임 오브젝트를 설정하여 시각적으로 강조되도록 합니다.
        EventSystem.current.SetSelectedGameObject(currentSelectable.gameObject);
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

    private void OnEnable()
    {
        SelectorController.SelectorEvent += InitializeNavigation;
    }
    private void OnDisable()
    {
        SelectorController.SelectorEvent -= InitializeNavigation;
    }
}

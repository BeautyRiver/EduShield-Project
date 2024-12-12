using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class ButtonKeyBoardSelector : MonoBehaviour
{
    public static ButtonKeyBoardSelector instance;


    [SerializeField] private List<Selectable> selectables = new List<Selectable>();
    [SerializeField] private int currentIndex = 0;

    private Dictionary<Selectable, Vector3> originalScales = new Dictionary<Selectable, Vector3>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
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
        selectables.AddRange(buttonsParents.GetComponentsInChildren<Selectable>());
        selectables.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

        originalScales.Clear(); // 기존 데이터 초기화
        foreach (var selectable in selectables)
        {
            originalScales[selectable] = selectable.transform.localScale; // 초기 스케일 저장
        }

        if (selectables.Count > 0)
        {
            currentIndex = 0;
            SelectCurrent();
        }
    }
   
    // 방향키로 ui 선택
    private void OnNavigate(InputValue inputValue)
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.isGameActive)
                return;
        }

        Vector2 input = inputValue.Get<Vector2>();
        if (input.y > 0 || input.x < 0)
        {
            MovePrevious();
        }
        else if (input.y < 0 || input.x > 0)
        {
            MoveNext();
        }
    }

    private void MoveNext()
    {
        currentIndex = (currentIndex + 1) % selectables.Count;
        SelectCurrent();
    }

    private void MovePrevious()
    {
        currentIndex = (currentIndex - 1 + selectables.Count) % selectables.Count;
        SelectCurrent();
    }

    private void SelectCurrent()
    {
        ResetAllButtonScales(); // 모든 버튼 크기 초기화

        var currentSelectable = selectables[currentIndex];
        currentSelectable.transform.DOScale(originalScales[currentSelectable] * 1.1f, 0.1f) // 기존 스케일 기준 크기 증가
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        currentSelectable.Select();
    }

    private void ResetAllButtonScales()
    {
        foreach (var selectable in selectables)
        {
            if (originalScales.TryGetValue(selectable, out Vector3 originalScale))
            {
                selectable.transform.DOScale(originalScale, 0.1f) // 초기 스케일로 복원
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
            }
        }
    }
}

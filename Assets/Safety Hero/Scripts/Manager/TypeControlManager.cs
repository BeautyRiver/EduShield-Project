using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TypeControlManager : MonoBehaviour
{
    [Header("# 스테이지별 타입 이미지들")]
    [SerializeField] private List<StageTypeImages> stageTypes = new List<StageTypeImages>();

    [Header("# UI이미지 설정")]
    [SerializeField] private List<Image> equipImages;
    [SerializeField] private List<Image> coolDownImages;

    [SerializeField] private bool[] currentTypeState; // 현재 장착한 무기
    [SerializeField] private float swapDelay = 3f; // 타입 스왑 딜레이
    [SerializeField] private float swapTimer; // 타입 스왑 딜레이 타이머
    [field: SerializeField] public int TypeIndex { get; private set; } // 현재 선택된 타입 인덱스
    private int spriteCount; 

    private void Awake()
    {
        currentTypeState = new bool[] { true, false, false, false, false };
        swapTimer = swapDelay;
        TypeIndex = -1; // 기본 무기 = 0번무기

        Initialize();
        CoolDownImageChangeFillAmount();
    }
    private void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;

        swapTimer = Mathf.Max(swapTimer - Time.deltaTime, 0f);

        if ((Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4) ||
            Input.GetKeyDown(KeyCode.Alpha5)) && swapTimer <= 0)
        {
            // 이전 무기 인덱스를 저장
            int previousTypeIndex = TypeIndex;

            if (Input.GetKeyDown(KeyCode.Alpha1)) TypeIndex = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) TypeIndex = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) TypeIndex = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) TypeIndex = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha5)) TypeIndex = 4;

            // 현재 선택한 타입이 이전과 동일하면 변신 해제
            if (TypeIndex == previousTypeIndex)
            {
                TypeIndex = -1;
            }
            if (TypeIndex > spriteCount)
            {
                TypeIndex = previousTypeIndex;
                return;
            }

            // 타입 스왑이 발생했을 때 UI 업데이트 이벤트 호출
            CoolDownImageChangeFillAmount();
            StartCoroutine(GameManager.instance.player.TransformationColor(TypeIndex));

            // 스왑 타이머 리셋
            swapTimer = swapDelay;
        }
    }
  
    private void CoolDownImageChangeFillAmount()
    {
        // 모든 쿨다운 이미지를 초기화
        foreach (var item in coolDownImages)
        {
            item.fillAmount = 1f;
        }

        int coolDownImagesCount = coolDownImages.Count;
        for (int i = 0; i < coolDownImagesCount; i++)
        {
            if (i != TypeIndex)
            {
                // 지역 변수로 i 값을 고정
                int index = i;

                coolDownImages[index].DOKill();
                // 안전한 범위 내에서만 Tween 실행
                coolDownImages[index].DOFillAmount(0f, swapDelay).OnComplete(() =>
                {
                    Vector3 originalScale = equipImages[index].transform.localScale;
                    equipImages[index].rectTransform.DOScale(originalScale * 1.2f, 0.05f).OnComplete(() =>
                    {
                        equipImages[index].DOFade(0.2f, 0.1f).SetLoops(2, LoopType.Yoyo);
                        equipImages[index].rectTransform.DOScale(originalScale, 0.1f);
                    });
                });
            }
        }
    }
    private void Initialize()
    {
        int childCount = transform.childCount;

        // 자식 오브젝트들을 미리 비활성화
        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        // 필요한 리스트의 크기를 미리 설정하여 성능 최적화
        equipImages.Capacity = childCount;
        coolDownImages.Capacity = childCount;

        // stageTypes[0]의 sprite 배열 크기만큼 반복
        spriteCount = stageTypes[0].sprite.Length;
        for (int i = 0; i < spriteCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);

            // GetComponentsInChildren을 한 번만 호출하여 필요한 이미지를 모두 가져옴
            Image[] images = child.GetComponentsInChildren<Image>(true);
            if (images.Length > 2) // 필요한 이미지가 2개 이상일 때만 추가
            {
                equipImages.Add(images[1]);
                coolDownImages.Add(images[2]);

                // 스프라이트 설정
                equipImages[i].sprite = stageTypes[0].sprite[i];
                coolDownImages[i].sprite = stageTypes[0].sprite[i];
            }
        }
    }
    [System.Serializable]
    public class StageTypeImages
    {
        public Sprite[] sprite;
    }
}

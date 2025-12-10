using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [Header("오른쪽 정보 패널")]
    [SerializeField] private GameObject rightStagePanel;
    [SerializeField] private Image right_stageImage;
    [SerializeField] private TextMeshProUGUI right_stageNameText;
    [SerializeField] private TextMeshProUGUI right_stageDifficultText;
    [SerializeField] private Image right_stageDifficultBackGround;
    [SerializeField] private Image right_stageDifficultImage;

    private List<StageItem> stageItems;
    private StageData currentSelectedStage;
    private void Awake()
    {
        rightStagePanel.SetActive(false); // 시작 시 우측 패널 비활성화
        stageItems = new List<StageItem>(GetComponentsInChildren<StageItem>());
        List<bool> isStageUnlocked = DataManager.instance.unlockedStages;

        for (int i = 0; i < stageItems.Count; i++)
        {
            StageItem currentItem = stageItems[i];
            currentItem.Setup(isStageUnlocked[i]);
            currentItem.GetComponentInChildren<Toggle>().onValueChanged.AddListener((isOn) =>
            {
                OnToggledMapItem(currentItem, isOn);
            }); // 토글 이벤트 리스너 추가
        }

    }

    public void OnToggledMapItem(StageItem item, bool isOn)
    {
        if (isOn)
        {
            StageData data = item.Data;

            if (currentSelectedStage == data)
                return;

            currentSelectedStage = data;

            rightStagePanel.SetActive(true);
            right_stageImage.sprite = data.stageImage;
            right_stageNameText.text = data.stageName;
            right_stageDifficultText.text = data.difficulty.ToString();
            right_stageDifficultImage.sprite = data.difficultyImage;
            right_stageDifficultBackGround.color = data.difficultyColor;
            //Debug.Log($"Selected Stage: {data.stageName}");
        }
    }

    public void OnStartGameButton()
    {
        if (currentSelectedStage != null)
        {
            LoadingSceneController.LoadScene("Game Scene");
        }
        else
        {
            Debug.LogWarning("No stage selected!");
        }
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage : MonoBehaviour, ISelectHandler
{
    [field: SerializeField]
    public StageData Data { get; private set; }

    [Header("UI 컴포넌트")]
    [SerializeField] private Image stageImage;
    [SerializeField] private TextMeshProUGUI stageDescText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private Toggle itemToggle;

    public void Setup(bool isUnlocked)
    {        
        stageImage.sprite = Data.stageImage;

        if (isUnlocked)
        {
            stageImage.color = Color.white;
            lockIcon.SetActive(false);
            itemToggle.interactable = true;
            stageDescText.text = Data.stageDesc;
        }
        else
        {
            lockIcon.SetActive(true);
            stageImage.color = new Color(1,1,1, 0.8f);
            itemToggle.interactable = false;
            stageDescText.text = "???";            
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        itemToggle.isOn = true;
    }
}

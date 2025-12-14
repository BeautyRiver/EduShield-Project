using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Result : MonoBehaviour
{
    [SerializeField] private GameObject[] resultUI;
    [SerializeField] private TextMeshProUGUI[] resultTexts;
    [SerializeField] private Image[] resultImage;
    [SerializeField] private GameObject buttons;

    private void Awake()
    {
        resultImage = new Image[resultUI.Length];
        resultTexts = new TextMeshProUGUI[resultUI.Length];
        for (int i = 0; i < resultUI.Length; i++)
        {
            resultImage[i] = resultUI[i].GetComponentInChildren<Image>(true);
            resultTexts[i] = resultUI[i].GetComponentInChildren<TextMeshProUGUI>(true);            
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < resultUI.Length; i++)
        {
            resultImage[i].color = new Color(1, 1, 1, 0);
            resultTexts[i].color = new Color(1, 1, 1, 0);
        }
        buttons.SetActive(true);
    }

    public void Lose()
    {
        resultUI[0].SetActive(true);
        resultImage[0].DOFade(1, 1f).SetUpdate(true);
        resultTexts[0].DOFade(1, 1f).SetUpdate(true);
    }

    public void Win()
    {
        resultUI[1].SetActive(true);
        resultImage[1].DOFade(1, 1f).SetUpdate(true);
        resultTexts[1].DOFade(1, 1f).SetUpdate(true);
    }
}

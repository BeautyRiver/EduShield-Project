using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Result : MonoBehaviour
{
    public GameObject[] resultUI;
    private TextMeshProUGUI[] resultTexts;
    private Image[] resultImage;

    private void Awake()
    {
        for(int i = 0; i < resultUI.Length; i++)
        {
            resultImage[i] = resultUI[i].GetComponentInChildren<Image>();
            resultTexts[i] = resultUI[i].GetComponentInChildren<TextMeshProUGUI>();            
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < resultUI.Length; i++)
        {
            resultImage[i].color = new Color(1, 1, 1, 0);
            resultTexts[i].color = new Color(1, 1, 1, 0);
        }
    }

    public void Lose()
    {
        resultImage[0].DOFade(1, 1f).SetUpdate(true);
        resultTexts[0].DOFade(1, 1f).SetUpdate(true);
    }

    public void Win()
    {
        resultImage[1].DOFade(1, 1f).SetUpdate(true);
        resultTexts[1].DOFade(1, 1f).SetUpdate(true);
    }
}

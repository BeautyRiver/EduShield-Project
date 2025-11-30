using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Gold, Time, Helath }
    public InfoType type;

    private TextMeshProUGUI myText;
    private Slider mySlider;

    private Tween healthTween;
    private Tween expTween;
    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();
    }

    public void UpdateHealth(float curHealth, float maxHealth)
    {
        if (type != InfoType.Helath) return;

        float targetValue = curHealth / maxHealth;

        healthTween?.Kill();

        healthTween = mySlider.DOValue(targetValue, 0.3f).SetEase(Ease.OutQuad);
    }

    public void UpdateExp(float curExp, float maxExp)
    {
        if (type != InfoType.Exp) return;

        float targetValue = curExp / maxExp;

        expTween?.Kill();
        expTween = mySlider.DOValue(targetValue, 0.2f).SetEase(Ease.Linear); 
    }


    private void LateUpdate()
    {
        switch (type)
        {

            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.playerLevel);
                break;

            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.playerKill);
                break;

            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
        }
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // 싱글톤으로 만들어서 어디서든 쉽게 접근하게 함
    public static HUDManager instance;

    [Header("UI References")]
    public Slider healthSlider;
    public Slider expSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI timeText;

    [Header("DOTween Settings")]
    public float barTweenDuration = 0.3f;

    private Tween healthTween;
    private Tween expTween;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float curHealth, float maxHealth)
    {
        float targetValue = curHealth / maxHealth;
        healthTween?.Kill(); // 기존 트윈 중지
        healthTween = healthSlider.DOValue(targetValue, barTweenDuration).SetEase(Ease.OutQuad);
    }

    public void UpdateExp(float curExp, float maxExp)
    {
        float targetValue = curExp / maxExp;
        expTween?.Kill();
        expTween = expSlider.DOValue(targetValue, barTweenDuration).SetEase(Ease.Linear);
    }

    public void UpdateLevel(int level)
    {
        levelText.text = string.Format("Lv.{0:F0}", level);
    }

    public void UpdateKill(int killCount)
    {
        killText.text = string.Format("{0:F0}", killCount);
    }

    public void UpdateTime(float gameTime, float maxGameTime)
    {
        float remainTime = maxGameTime - gameTime;
        int min = Mathf.FloorToInt(remainTime / 60);
        int sec = Mathf.FloorToInt(remainTime % 60);
        timeText.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }
}
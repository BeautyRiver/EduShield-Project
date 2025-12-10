using System; // Action을 쓰기 위해 필요
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Box : InteractableObject
{
    [Header("# 박스 정보")]
    public static int baseBoxCost = 50;
    public static int openCount = 0;
    private float costMultiplier = 1.15f;

    // [핵심 1] "박스가 열렸다"는 사실을 알리는 방송국(이벤트)
    public static Action OnBoxCostChanged;

    [SerializeField] private Image gold_sprite;
    [SerializeField] private TextMeshProUGUI gold_text;

    private void Start()
    { 
        OnBoxCostChanged += UpdatePriceText;
        UpdatePriceText();
    }

    private void OnDestroy()
    {
        // 박스가 사라질 때 구독 취소
        OnBoxCostChanged -= UpdatePriceText;
    }

    // 가격표 텍스트만 갱신하는 함수
    private void UpdatePriceText()
    {
        int cost = GetCurrentCost();
        gold_text.text = cost.ToString();

        // 돈 부족하면 빨갛게 바꾸는 로직
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            PlayerInGame playerStats = GameManager.instance.player.GetComponent<PlayerInGame>();
            bool canBuy = playerStats.gold >= cost;
            gold_text.color = canBuy ? Color.white : Color.red;
        }
    }

    private int GetCurrentCost()
    {
        float cost = baseBoxCost * Mathf.Pow(costMultiplier, openCount);
        return (int)cost;
    }

    public override void ShowInteractUi(bool show)
    {
        base.ShowInteractUi(show);
        if (show) UpdatePriceText();
    }

    public override void Interact()
    {
        PlayerInGame playerStats = GameManager.instance.player.GetComponent<PlayerInGame>();
        int cost = GetCurrentCost();

        if (playerStats.gold >= cost)
        {
            playerStats.IncreaseGold(-cost);

            openCount++;

            OnBoxCostChanged?.Invoke();

            Debug.Log($"박스 오픈! 소모 골드: {cost}");
            UIManager.instance.ShowRewardBox();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }
}
using UnityEngine;

// 1. 등급(Rarity) 정의
// 순서대로 정렬해두면 나중에 비교하기도 편해요 (Common < Legendary)
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

// 2. 확률 및 계산 로직을 담당할 정적 클래스
public static class Utils
{
    // 각 등급별 등장 확률 (합계가 100이 되도록 설정)
    // Common 70%, Uncommon 15%, Rare 10%, Epic 4%, Legendary 1% 로 예시를 잡았습니다.
    private static readonly float[] Probs = { 70f, 15f, 10f, 4f, 1f };

    // 각 등급별 강화 배율 (기획하신 내용 반영)
    // Common(1배) ~ Legendary(2배)
    private static readonly float[] Rates = { 1.0f, 1.2f, 1.4f, 1.6f, 2.0f };

    /// <summary>
    /// 확률에 따라 랜덤한 등급을 뽑아주는 함수
    /// </summary>
    public static Rarity GetRandomRarity()
    {
        // 0 ~ 100 사이의 랜덤 숫자 생성
        float randomValue = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < Probs.Length; i++)
        {
            cumulative += Probs[i];

            // 누적된 확률보다 랜덤 값이 작으면 해당 등급 당첨!
            if (randomValue <= cumulative)
            {
                return (Rarity)i;
            }
        }

        // 혹시나 계산 오차로 여기까지 오면 가장 낮은 등급 반환
        return Rarity.Common;
    }

    /// <summary>
    /// 등급에 맞는 배율을 반환하는 함수
    /// </summary>
    public static float GetRarityMultiplier(Rarity rarity)
    {
        // Enum을 int로 변환해서 배열 인덱스로 사용
        int index = (int)rarity;

        // 안전 장치: 인덱스 범위 체크
        if (index < 0 || index >= Rates.Length)
            return 1.0f;

        return Rates[index];
    }

    /// <summary>
    /// 등급에 따른 텍스트 색상 반환 (UI용)
    /// </summary>
    public static Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return Color.white;
            case Rarity.Uncommon: return Color.green;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return Color.magenta; // 보라색
            case Rarity.Legendary: return new Color(1f, 0.84f, 0f); // 금색
            default: return Color.white;
        }
    }
}
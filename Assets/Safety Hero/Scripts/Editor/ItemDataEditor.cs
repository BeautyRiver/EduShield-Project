using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    // Serialized Properties 선언
    SerializedProperty maxLevel;
    SerializedProperty itemCategory;
    SerializedProperty itemType;
    SerializedProperty itemId;
    SerializedProperty itemName;
    SerializedProperty itemDesc;
    SerializedProperty itemIcon;

    // 무기 속성
    SerializedProperty baseDamage;
    SerializedProperty baseCount;
    SerializedProperty basePer;
    SerializedProperty baseDelay;
    SerializedProperty baseSpeed;
    SerializedProperty baseScale;
    SerializedProperty prefab;
    SerializedProperty hand;

    // 기어 속성
    SerializedProperty damages;
    SerializedProperty counts;
    SerializedProperty pers;
    SerializedProperty gearRates;
    private void OnEnable()
    {
        // 아이템 속성 로드
        maxLevel = serializedObject.FindProperty("maxLevel");
        itemCategory = serializedObject.FindProperty("itemCategory");
        itemType = serializedObject.FindProperty("itemType");
        itemId = serializedObject.FindProperty("itemId");
        itemName = serializedObject.FindProperty("itemName");
        itemDesc = serializedObject.FindProperty("itemDesc");
        itemIcon = serializedObject.FindProperty("itemIcon");

        // 무기 속성 로드
        baseDamage = serializedObject.FindProperty("baseDamage");
        baseCount = serializedObject.FindProperty("baseCount");
        basePer = serializedObject.FindProperty("basePer");
        baseDelay = serializedObject.FindProperty("baseDelay");
        baseSpeed = serializedObject.FindProperty("baseSpeed");
        baseScale = serializedObject.FindProperty("baseScale");
        prefab = serializedObject.FindProperty("prefab");
        hand = serializedObject.FindProperty("hand");

        // 기어 속성 로드
        damages = serializedObject.FindProperty("damages");
        gearRates = serializedObject.FindProperty("gearRates");
        counts = serializedObject.FindProperty("counts");
        pers = serializedObject.FindProperty("pers");
    }

    public override void OnInspectorGUI()
    {
        // 인스펙터 업데이트
        serializedObject.Update();

        // itemCategory에 따른 필드 구분
        ItemData.ItemCategory category = (ItemData.ItemCategory)itemCategory.enumValueIndex;

        // itemType을 enum의 실제 값(int)으로 가져와 분류
        int itemTypeValue = itemType.intValue;
        ItemData.ItemType type = (ItemData.ItemType)itemTypeValue;  // int 값을 enum으로 변환

        // 아이템 속성 표시
        EditorGUILayout.PropertyField(itemCategory);
        EditorGUILayout.PropertyField(itemType);
        EditorGUILayout.PropertyField(itemId);

        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemDesc);
        EditorGUILayout.PropertyField(itemIcon);               
        
        if (category == ItemData.ItemCategory.Weapon)
        {
            // 무기 관련 속성만 표시
            EditorGUILayout.LabelField("무기 속성", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(maxLevel);
            EditorGUILayout.PropertyField(baseDamage);
            EditorGUILayout.PropertyField(baseCount);
            EditorGUILayout.PropertyField(basePer);
            EditorGUILayout.PropertyField(baseDelay);
            EditorGUILayout.PropertyField(baseSpeed);
            EditorGUILayout.PropertyField(baseScale);

            EditorGUILayout.PropertyField(damages);
            EditorGUILayout.PropertyField(counts);
            EditorGUILayout.PropertyField(pers);
            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(hand);
        }
        else if (category == ItemData.ItemCategory.Gear)
        {
            // 기어 관련 속성만 표시
            EditorGUILayout.LabelField("기어 속성", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(maxLevel);
            EditorGUILayout.PropertyField(gearRates);

        }
        else if (category == ItemData.ItemCategory.Etc)
        {
            // 기타 카테고리일 때 추가할 항목이 있다면 여기에 추가 가능
            EditorGUILayout.LabelField("기타 아이템", EditorStyles.boldLabel);
        }

        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }
}


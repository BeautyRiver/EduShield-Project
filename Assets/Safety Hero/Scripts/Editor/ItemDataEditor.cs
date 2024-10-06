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
    SerializedProperty baseDamageInterval;
    SerializedProperty baseSpeed;
    SerializedProperty baseRange;
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
        baseDamageInterval = serializedObject.FindProperty("baseDamageInterval");
        baseSpeed = serializedObject.FindProperty("baseSpeed");
        baseRange = serializedObject.FindProperty("baseRange");
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

        // 아이템 속성 표시
        EditorGUILayout.PropertyField(itemCategory);
        EditorGUILayout.PropertyField(itemType);
        EditorGUILayout.PropertyField(itemId);
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemDesc);
        EditorGUILayout.PropertyField(itemIcon);

        // itemCategory에 따른 필드 구분
        ItemData.ItemCategory category = (ItemData.ItemCategory)itemCategory.enumValueIndex;

        if (category == ItemData.ItemCategory.Weapon)
        {
            GUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("무기 속성", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(maxLevel);
            EditorGUILayout.PropertyField(baseDamage);
            EditorGUILayout.PropertyField(baseDamageInterval);
            EditorGUILayout.PropertyField(baseCount);
            EditorGUILayout.PropertyField(basePer);
            EditorGUILayout.PropertyField(baseDelay);
            EditorGUILayout.PropertyField(baseSpeed);
            EditorGUILayout.PropertyField(baseRange);
            EditorGUILayout.PropertyField(baseScale);

            EditorGUILayout.PropertyField(damages);
            EditorGUILayout.PropertyField(counts);
            EditorGUILayout.PropertyField(pers);
            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(hand);
            GUILayout.EndVertical();
        }
        else if (category == ItemData.ItemCategory.Gear)
        {
            GUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("기어 속성", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(maxLevel);
            EditorGUILayout.PropertyField(gearRates);
            GUILayout.EndVertical();
        }
        else if (category == ItemData.ItemCategory.Etc)
        {
            GUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("기타 아이템", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }
}

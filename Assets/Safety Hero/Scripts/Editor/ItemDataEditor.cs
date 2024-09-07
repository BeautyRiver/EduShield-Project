using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    // Serialized Properties 선언
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
    SerializedProperty prefab;
    SerializedProperty hand;

    // 기어 속성
    SerializedProperty damages;
    SerializedProperty weaponSpeeds;
    SerializedProperty counts;
    SerializedProperty pers;
    SerializedProperty speeds;

    private void OnEnable()
    {
        // 아이템 속성 로드
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
        prefab = serializedObject.FindProperty("prefab");
        hand = serializedObject.FindProperty("hand");

        // 기어 속성 로드
        damages = serializedObject.FindProperty("damages");
        weaponSpeeds = serializedObject.FindProperty("weaponSpeeds");
        counts = serializedObject.FindProperty("counts");
        pers = serializedObject.FindProperty("pers");
        speeds = serializedObject.FindProperty("speeds");
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
        ItemData.ItemType type = (ItemData.ItemType)itemCategory.enumValueIndex;

        if (category == ItemData.ItemCategory.Weapon)
        {
            // 무기 관련 속성만 표시
            EditorGUILayout.LabelField("무기 속성", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(baseDamage);
            EditorGUILayout.PropertyField(baseCount);
            EditorGUILayout.PropertyField(basePer);
            EditorGUILayout.PropertyField(baseDelay);
            EditorGUILayout.PropertyField(baseSpeed);
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
            EditorGUILayout.PropertyField(damages);
            EditorGUILayout.PropertyField(weaponSpeeds);
            EditorGUILayout.PropertyField(counts);
            EditorGUILayout.PropertyField(pers);
            EditorGUILayout.PropertyField(speeds);
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

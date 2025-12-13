using VInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Cecil;


public class ForDebug : MonoBehaviour
{
    [Header("# Item Datas")]
    public Data[] itemData;
    public GameObject itmeGroup;
    public GameObject stageItemPrefab;
    [ReadOnly] public LevelUpItemSetting[] levelItemSet;

    [Header("# Debug Text")]
    public TextMeshProUGUI debugText;


    private bool isInvinsible;
    private bool is2xSpeed;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var player = GameManager.instance.player;
            int maxExp = player.nextExp[Mathf.Min(player.level, player.nextExp.Length - 1)];
            player.GetExp(maxExp);
        }
    }
    [Button]
    public void InitializeItems()
    {
        levelItemSet = itmeGroup.GetComponentsInChildren<LevelUpItemSetting>(true);

        for (int i = 0; i < levelItemSet.Length; i++)
        {
            DestroyImmediate(levelItemSet[i].gameObject);
        }

        levelItemSet = new LevelUpItemSetting[itemData.Length];
        for (int i = 0; i < itemData.Length; i++)
        {
            levelItemSet[i] = Instantiate(stageItemPrefab, itmeGroup.transform).GetComponent<LevelUpItemSetting>();
            levelItemSet[i].data = itemData[i];            
            // levelItemSet[i].GetComponent<Button>().onClick.AddListener(levelItemSet[i].OnClick);
            levelItemSet[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            levelItemSet[i].gameObject.SetActive(true);
        }
    }
}

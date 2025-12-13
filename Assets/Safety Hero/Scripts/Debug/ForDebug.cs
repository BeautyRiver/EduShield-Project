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


    private bool isInvincible;
    private bool is2xSpeed;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            var player = GameManager.instance.player;
            int maxExp = GameManager.instance.nextExp[Mathf.Min(player.level, GameManager.instance.nextExp.Count - 1)];
            player.GetExp(maxExp);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            isInvincible = !isInvincible;
            GameManager.instance.player.isInvincible = isInvincible;
            debugText.text = isInvincible ? "무적 ON" : "무적 OFF";
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GlobalManager.instance.nowTimeScale += 0.5f;
            Time.timeScale = GlobalManager.instance.nowTimeScale;
            debugText.text = "시간속도: " + GlobalManager.instance.nowTimeScale + "X";
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GlobalManager.instance.nowTimeScale = Mathf.Max(0.5f, GlobalManager.instance.nowTimeScale - 0.5f);
            Time.timeScale = GlobalManager.instance.nowTimeScale;
            debugText.text = "시간속도: " + GlobalManager.instance.nowTimeScale + "X";
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

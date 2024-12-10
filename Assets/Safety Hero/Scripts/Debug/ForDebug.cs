using VInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ForDebug : MonoBehaviour
{
    public Transform itemParent;
    public GameObject sampleItem;
    public Sprite[] uiPanelImages;
    public ItemSetting[] items;
    public DataGuide[] itemData;

    public TextMeshProUGUI debugText;
    private bool isInvinsible;
    private bool is2xSpeed;

    private void Update()
    {
#if UNITY_EDITOR
        if (!GameManager.instance.isGameActive)
            return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!isInvinsible)
            {
                isInvinsible = true;
                GameManager.instance.player.health = 99999999999;
                debugText.text = "무적모드";
            }
            else
            {
                isInvinsible = false;
                GameManager.instance.player.health = 100;
                debugText.text = "무적 해제";
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameManager.instance.player.playerMove.currentSpeed += 1f;
            debugText.text = "스피드 증가 => " + GameManager.instance.player.playerMove.currentSpeed;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.instance.player.playerMove.currentSpeed -= 1f;
            debugText.text = "스피드 감소 => " + GameManager.instance.player.playerMove.currentSpeed;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            //is2xSpeed = true;
            GameManager.instance.nowTimeScale += 0.5f;
            Time.timeScale = GameManager.instance.nowTimeScale;
            debugText.text = $"+0.5 배속 / Now TimeScale: {GameManager.instance.nowTimeScale.ToString()}";
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameManager.instance.nowTimeScale -= 0.5f;
            Time.timeScale = GameManager.instance.nowTimeScale;
            debugText.text = $"-0.5 배속 / Now TimeScale: {GameManager.instance.nowTimeScale.ToString()}";
        }
        // 레벨업
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameManager.instance.GetExp(GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)]); // 레벨업 
        }
#endif
    }

    [Button]
    public void InitializeItems()
    {
        int childCount = itemParent.childCount;
        Debug.Log($"childCount : {childCount}, itemDataLength : {itemData.Length}");

        if (childCount < itemData.Length)
        {
            for (int i = childCount; i < itemData.Length; i++)
            {
                GameObject newItem = Instantiate(sampleItem, itemParent);                         
            }
        }
        items = itemParent.GetComponentsInChildren<ItemSetting>();
        int idx = 0;

        foreach (ItemSetting item in items)
        {
            item.itemData = itemData[idx];
            item.gameObject.name = itemData[idx].name;
            item.transform.GetChild(1).GetComponent<Image>().sprite = itemData[idx].itemIcon;
            if (item.itemData is BulletData)
            {
                item.GetComponent<Image>().sprite = uiPanelImages[0];

            }
            else if (item.itemData is GearData)
            {
                item.GetComponent<Image>().sprite = uiPanelImages[1];

            }
            else if (item.itemData is EtcData)
            {
                item.GetComponent<Image>().sprite = uiPanelImages[2];

            }         
            idx++;            
        }

    }
}

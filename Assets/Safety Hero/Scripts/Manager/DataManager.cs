using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VInspector;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    [field: SerializeField] public int selectedCharacterId { get; private set; }
    [field: SerializeField] public int gold { get; private set; }
    [field: SerializeField] public List<bool> unlockedStages;
    [field: SerializeField] public int totalStageCount { get; private set; } = 3;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializedData(Dictionary<string, object> userData)
    {
        if (userData != null)
        {
            this.selectedCharacterId = Convert.ToInt32(userData["selectedCharacterId"]);
            this.gold = Convert.ToInt32(userData["gold"]);
            if (userData.TryGetValue("unlockedStages", out object stagesObj) && stagesObj is List<object> stageList)
            {
                this.unlockedStages = new List<bool>();
                foreach(object item in stageList)
                {
                    this.unlockedStages.Add(Convert.ToBoolean(item));
                }
            }
            else
            {
                Debug.LogWarning("unlockedStages 데이터가 없거나 형식이 올바르지 않습니다. 기본값으로 초기화합니다.");
                InitalizeNewUserSetting();
            }           
        }
        else
        {
            InitalizeNewUserSetting();
        }

        Debug.Log(
                $"게임 데이터 로드 완료: 선택된 캐릭터 ID = {this.selectedCharacterId}," +
                $" 골드 = {this.gold}," +
                $" 잠금 해제된 스테이지 수 = {this.unlockedStages.Count}");
    }

    private void InitalizeNewUserSetting()
    {
        this.selectedCharacterId = 0;
        this.gold = 0;
        this.unlockedStages = new List<bool>(totalStageCount);
        this.unlockedStages.Add(true); // 첫 스테이지는 잠금 해제
        for (int i = 1; i < totalStageCount; i++)
        {
            this.unlockedStages.Add(false); // 나머지 스테이지는 잠금
        }
    }

    [Button("SaveData")]
    public async Task SaveGameData()
    {
        var dataToSave = new Dictionary<string, object>
        {
            { "selectedCharacterId", this.selectedCharacterId   },
            { "gold", this.gold },
            { "unlockedStages", this.unlockedStages }
        };

        try
        {
            await FirebaseManager.Instance.SaveUserData(dataToSave);            
        }
        catch (Exception e)
        {
            Debug.LogError($"게임 데이터 저장 실패: {e.Message}");
        }
    }

    
}

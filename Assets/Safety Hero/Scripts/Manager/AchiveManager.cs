using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice; // 안내 메시지
    
    private enum Achive { UnlockChar1, UnlockChar2 } // 업적
    private Achive[] achives;

    private WaitForSecondsRealtime wait; // time.scale에 영향 안받게
    private void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);
        // MyData 데이터가 없다면 초기화 시작
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    private void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    private void Start()
    {
        UnlockCharacter();
    }

    private void UnlockCharacter()
    {
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string aciveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(aciveName) == 1;
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }
    private void LateUpdate()
    {
        foreach (Achive chive in achives)
        {
            CheckAchive(chive);
        }
    }

    private void CheckAchive(Achive achive)
    {
        bool isAchive = false;
        switch (achive)
        {
            case Achive.UnlockChar1:
                isAchive = GameManager.instance.kill >= 10;
                break;
            case Achive.UnlockChar2:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0 )
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);
            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }
            StartCoroutine(NoticeRoutine());
        }
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); // 음향재생

        yield return wait;

        uiNotice.SetActive(false);
    }
}


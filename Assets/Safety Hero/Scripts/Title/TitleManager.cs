using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject characterSelect; // 캐릭터 선택 창

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // 씬 전환 설정
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    // 캐릭터 선택창 관리
    public void CharcterSelecter(bool turnOn)
    {
        if (turnOn)
        {
            characterSelect.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);        
        }
        else if (!turnOn)
        {
            characterSelect.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        }
    }

    #region 캐릭터 버튼 관리
    public void SelectCharacter(PlayerData playerData)
    {
        DataManager.instance.currentPlayerData = playerData;        
        LoadScene("Game Scene");
    }
    #endregion

}

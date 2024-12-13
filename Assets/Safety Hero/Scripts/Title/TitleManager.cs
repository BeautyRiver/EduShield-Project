using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{    
    public static PlaylistController playlistController;  

    private void Start()
    {
        // 플레이리스트 설정
        playlistController = MasterAudio.OnlyPlaylistController;
        if (playlistController.CurrentPlaylist.playlistName != "Title Bgm")
            MasterAudio.ChangePlaylistByName("Title Bgm");
        else
            MasterAudio.StartPlaylist("Title Bgm");

        //ButtonKeyBoardSelector.instance.InitializeNavigation(titleFirstSelectButton); // 키보드로 선택가능한 버튼들 할당      
    } 

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


    /* public void SelectCharacter(PlayerData playerData)
     {
         DataManager.instance.currentPlayerData = playerData;
         SceneManager.LoadScene(idx + 1);
     }*/

}

using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VInspector;

public class TitleManager : MonoBehaviour
{
    [Foldout("처음 선택되는 버튼들")]
    [SerializeField] private Selectable titleFirstSelectedButton; // 타이틀 씬에서 처음 선택되는 버튼
    [SerializeField] private Selectable optionFirstSelectedButton; // 옵션 씬에서 처음 선택되는 버튼


    public static PlaylistController playlistController;  

    private void Start()
    {        
        InitTitle();
    }

    private void InitTitle()
    {
        playlistController = MasterAudio.OnlyPlaylistController;
        if (playlistController.CurrentPlaylist.playlistName != "Title Bgm")
            MasterAudio.ChangePlaylistByName("Title Bgm");
        else
            MasterAudio.StartPlaylist("Title Bgm");

        SetTitleFirstButton();
    }

    public void SetTitleFirstButton()
    {
        titleFirstSelectedButton.Select();
    }

    public void SetOptionFirstButton()
    {
        optionFirstSelectedButton.Select();
    }



    // 게임 종료
    public void GameQuit()
    {
        FirebaseManager.Instance.Logout();
        SceneManager.LoadScene("Login");
    }


}

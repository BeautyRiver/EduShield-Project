using DarkTonic.MasterAudio;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        MasterAudio.StartPlaylist("Title");
    }
    public void StartButton()
    {
        LoadingSceneController.LoadScene("Lobby Scene");
    }
}

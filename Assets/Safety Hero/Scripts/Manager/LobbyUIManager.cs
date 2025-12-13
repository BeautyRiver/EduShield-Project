using System;
using UnityEngine;


public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager instance;
    [SerializeField] private GameObject stageSelectUi;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Destroy(this.gameObject);
    }
    public void SwitchStageSelectUi()
    {
        bool isActive = stageSelectUi.activeSelf;
        stageSelectUi.SetActive(!isActive);

        if (isActive == true) // UI가 닫힐 때
        {
            GlobalManager.instance.ChangePlayerState(PlayerState.FreeMove);
            LobbyManager.instance.DeInteractingCamera();
        }
    }

}

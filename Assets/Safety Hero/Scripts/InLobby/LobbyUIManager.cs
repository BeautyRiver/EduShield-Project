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
        if (isActive == true)
        {
            stageSelectUi.SetActive(false);
            LobbyManager.instance.ChangeState(LobbyState.FreeMoving);
        }
        else
        {
            stageSelectUi.SetActive(true);
        }
    }

}

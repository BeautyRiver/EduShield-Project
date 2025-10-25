using System;
using UnityEngine;


public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject selectMapUi;      


    public void OpenSelectMapUi()
    {
        selectMapUi.SetActive(!selectMapUi.activeSelf);
    }

}

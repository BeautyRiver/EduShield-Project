using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectorManager : MonoBehaviour
{
    public static SelectorManager instance;
    [SerializeField] private GameObject currentSelectedButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    void Update()
    {
        // GameManager가 없거나, GameManager가 존재하지만 isGameActive가 true일 때 처리
        if (GameManager.instance == null || GameManager.instance.isGameActive)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                currentSelectedButton = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                Debug.Log("currentSelectedButton is null");
                EventSystem.current.SetSelectedGameObject(currentSelectedButton);
            }
        }
    }

}

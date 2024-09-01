using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControll : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.GameStart(GameManager.instance.playerData.characterId);
    }
}

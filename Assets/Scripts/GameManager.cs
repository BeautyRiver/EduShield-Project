using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("게임 시스템 관리")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("외부 참조")]
    public PoolManager pool;
    public Player player;

    private void Awake()
    {
        instance = this;            
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }
}

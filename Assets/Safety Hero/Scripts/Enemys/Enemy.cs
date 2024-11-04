using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Default,
        Uniqe,
        MiniBoss
    }
    [Header("# 공통 속성")]   
    public EnemyType enemyType;
    public int id;
    public float damage;
    public float maxHealth;
    public float health;
    public float speed;
    public int exp;
    public bool isLive;
    protected Vector2 nextVec;

    [Header("# 참조")]
    [SerializeField] protected TypeControlManager typeControlManager;
    [SerializeField] protected RuntimeAnimatorController[] animCon;    

    protected Rigidbody2D target;
    protected Collider2D coll;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriter;
    protected Animator anim;
    protected SortingGroup sortingGroup;
    protected GameManager gm;

    protected virtual void Awake()
    {
        // 초기 할당        
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();

        target = gm.player.GetComponent<Rigidbody2D>();
        gm = GameManager.instance;
    }
    public void Init(SpawnData data)
    {

    }

    public void DamagedLogic(Collider2D a, float b)
    {

    }

}

using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [Header("# 참조")]
    [SerializeField]
    protected TypeControlManager typeControlManager;

    [SerializeField]
    protected RuntimeAnimatorController[] animCon;

    [SerializeField]
    protected Rigidbody2D target;

    protected Collider2D coll;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriter;
    protected Animator anim;
    protected SortingGroup sortingGroup;
    protected GameManager gm;
    protected Vector2 nextVec;

    protected virtual void Awake()
    {
        // 초기 할당
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        gm = GameManager.instance;
    }

   
}

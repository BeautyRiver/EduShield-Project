using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    [SerializeField] private Sprite[] expImages;
    [SerializeField] private SpriteRenderer SpriteRenderer;

    [SerializeField] private bool isMoving = false;
    [SerializeField] private int expValue;
    [SerializeField] private float expMoveSpeed;

    public int exp
    {
        get { return expValue; }
        set
        {
            if (value >= 0)
                expValue = value;
        }
    }
    public bool IsMoving { get; set; }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(SetSpriteByExp());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.instance.isGameActive)
            return;

        if (collision.CompareTag("Player"))
        {
            MasterAudio.PlaySound("Coin"); // Coin 사운드 재생
            GameManager.instance.GetExp(exp);
            gameObject.SetActive(false);
            IsMoving = false;
        }
    }
    private IEnumerator SetSpriteByExp()
    {
        yield return null;
        if (expValue >= 5)
            SpriteRenderer.sprite = expImages[2];
        else if (expValue >= 3)
            SpriteRenderer.sprite = expImages[1];
        else if (expValue >= 1)
            SpriteRenderer.sprite = expImages[0];
    }


    public void ItemMoveLogic(Transform playerPos)
    {
        IsMoving = true;
        // 플레이어와 반대 방향 계산
        Vector2 directionAwayFromPlayer = (transform.position - playerPos.position).normalized;
        Vector2 targetPosition = transform.position + (Vector3)directionAwayFromPlayer * 0.5f;  // 반대 방향으로 약간 이동

        // DOTween을 사용해 플레이어 반대 방향으로 살짝 이동
        transform.DOMove(targetPosition, 0.15f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // 반대 방향으로 이동이 끝나면 플레이어에게 따라가는 코루틴 시작
            if (gameObject.activeSelf == true)
                StartCoroutine(FollowPlayer(playerPos));
        });

    }
    private IEnumerator FollowPlayer(Transform playerPos)
    {
        float closeDistance = 0.1f;  // 플레이어에게 충분히 가까워졌는지 판단할 거리
        while (Vector2.Distance(transform.position, playerPos.position) > closeDistance)
        {
            // 플레이어의 현재 위치를 향해 경험치 아이템이 이동
            Vector2 direction = (playerPos.position - transform.position).normalized;
            transform.Translate(direction * expMoveSpeed * Time.deltaTime);  // 경험치 이동 속도 조절
            yield return null;  // 다음 프레임까지 대기
        }
    }
}

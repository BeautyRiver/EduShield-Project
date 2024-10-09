using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DarkTonic.MasterAudio;  // DOTween 네임스페이스 추가

public class IntroManager : MonoBehaviour
{
    // 텍스트 관련 변수들
    [Header("텍스트 설정")]
    [SerializeField] private TextMeshProUGUI scriptText;  // TextMeshProUGUI 컴포넌트
    [SerializeField] private RectTransform sciprtBar; // ScriptBar Transform
    [SerializeField] private Animator announcerAnim;
    [TextArea]
    [SerializeField] private string[] scripts;  // 출력할 텍스트 배열
    [SerializeField] private float typingSpeed = 0.05f;  // 텍스트 타이핑 속도
    [SerializeField] private int currentScriptIndex = 0;  // 현재 출력 중인 텍스트의 인덱스
    private bool isTextComplete = false;  // 현재 텍스트가 모두 출력되었는지 여부
    private bool isScriptEnd = false; // 제공된 스크립트 종료 여부
    private Tweener typingTween;  // DOTween 애니메이션 저장 변수
    private bool isTextSkipOk;

    // 타자 소리 관련 변수들
    [Header("타자 소리 설정")]
    private float typeSoundInterval = 0.1f;  // 타자기 소리 간격
    private float timeSinceLastTypeSound = 0f;  // 마지막 타자기 소리가 난 후 경과 시간

    // UI 관련 변수들
    [Header("UI 설정")]
    [SerializeField] private Image fadeImage; // 페이드용 이미지
    [SerializeField] private GameObject skipOption; // 스킵 옵션 UI
    [SerializeField] private GameObject scriptArrow; // ScriptBar에 위치한 화살표 
    [SerializeField] private Image newsImage; // 뉴스 이미지
    [SerializeField] private Sprite heroImage; // 히어로 이미지
    [SerializeField] private Sprite[] newsImages; // 뉴스 이미지에 사용할 이미지들

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        isTextSkipOk = true;
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(0, 1f).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
            StartCoroutine(ScriptBarOnCorutin());
        });
    }

    private void Update()
    {
        // 스페이스바를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Space) && !isScriptEnd && isTextSkipOk)
        {
            // 타이핑 중인 텍스트가 있으면 즉시 완료
            if (typingTween != null && typingTween.IsPlaying())
            {
                typingTween.Complete();  // 텍스트 타이핑 즉시 완료
            }
            // 텍스트가 이미 다 출력되었으면 다음 텍스트 출력
            else if (isTextComplete)
            {
                MasterAudio.PlaySound("NextChat");
                DisplayNextScript();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0)
        {
            MasterAudio.PlaySound("BtnClick");
            Time.timeScale = 0;
            skipOption.SetActive(true);
        }
    }
    // 다음 스크립트를 출력하는 함수
    private void DisplayNextScript()
    {
        if (currentScriptIndex >= scripts.Length)
        {
            scriptArrow.SetActive(false);
            isTextSkipOk = false;
            fadeImage.gameObject.SetActive(true);
            fadeImage.DOFade(1, 1f).OnComplete(() =>
            {
                LoadingSceneController.LoadScene("Title Scene");
            });
        }
        else
        {
            announcerAnim.SetBool("isTalk", true);

            isTextComplete = false;  // 새로운 텍스트 출력이 시작되었으므로 완료 상태를 false로 설정
            scriptText.text = "";  // 텍스트 초기화

            // DOText를 이용하여 텍스트 타이핑 효과 시작
            typingTween = scriptText.DOText(scripts[currentScriptIndex], typingSpeed * scripts[currentScriptIndex].Length)
                .SetEase(Ease.Linear)
                .OnUpdate(() => PlayTypingSound())
                .OnComplete(() =>
                {
                    isTextComplete = true;
                    announcerAnim.SetBool("isTalk", false);
                });  // 텍스트가 다 출력되면 isTextComplete를 true로 설정

            currentScriptIndex++;  // 다음 텍스트로 이동

            if (currentScriptIndex == 2)
            {
                isTextSkipOk = false;
                scriptArrow.SetActive(false);
                StartCoroutine(ChangeNewsImageCorutin());
            }
            if (currentScriptIndex == 5)
            {
                // 세이프티 히어로 등장
                isTextSkipOk = false;
                scriptArrow.SetActive(false);
                StartCoroutine(ChangeHeroImage());
            }
        }
    }
    IEnumerator ScriptBarOnCorutin()
    {
        yield return new WaitForSeconds(0.5f);
        sciprtBar.DOAnchorPos(Vector3.zero, 0.5f).OnComplete(() =>
        {
            DisplayNextScript();
        });
    }

    IEnumerator ChangeHeroImage()
    {
        yield return new WaitForSeconds(1f);
        newsImage.sprite = heroImage;
        Camera.main.transform.DOShakePosition(1f);
        yield return new WaitForSeconds(2f);
        scriptArrow.SetActive(true);
        isTextSkipOk = true;
    }
    IEnumerator ChangeNewsImageCorutin()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < newsImages.Length; i++)
        {
            newsImage.sprite = newsImages[i];
            yield return new WaitForSeconds(1f);
        }
        yield return null;
        scriptArrow.SetActive(true);
        isTextSkipOk = true;
    }

    // 타이핑 중 일정 간격마다 타자기 소리를 재생하는 함수
    private void PlayTypingSound()
    {
        // 일정 시간이 지난 후에만 타자기 소리 재생
        if (Time.time - timeSinceLastTypeSound >= typeSoundInterval)
        {
            MasterAudio.PlaySound("ChatSound");  // 타자기 소리 재생
            timeSinceLastTypeSound = Time.time;  // 마지막 소리 재생 시간 업데이트
        }
    }
    public void Skip()
    {
        CloseOption();
        fadeImage.DOFade(1, 0.5f).OnComplete(() =>
        {
            LoadingSceneController.LoadScene("Title Scene");
        });
    }
    public void CloseOption()
    {
        skipOption.SetActive(false);
        Time.timeScale = 1;
    }
}

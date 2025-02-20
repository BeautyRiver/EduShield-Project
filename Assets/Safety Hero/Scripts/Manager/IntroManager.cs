using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using VInspector;  // DOTween 네임스페이스 추가

public class IntroManager : MonoBehaviour
{
    // 텍스트 관련 변수들
    [Tab("텍스트 설정")]
    [SerializeField] private TextMeshProUGUI scriptText;  // TextMeshProUGUI 컴포넌트
    [SerializeField] private RectTransform sciprtBar; // ScriptBar Transform
    [SerializeField] private Animator announcerAnim;
    [TextArea]
    [SerializeField] private string[] scripts;  // 출력할 텍스트 배열
    [SerializeField] private float typingSpeed = 0.05f;  // 텍스트 타이핑 속도
    [SerializeField] private int currentScriptIndex = 0;  // 현재 출력 중인 텍스트의 인덱스
    private bool isTextComplete = false;  // 현재 텍스트가 모두 출력되었는지 여부
    private bool isScriptEnd = false; // 제공된 스크립트 종료 여부
    private bool isTextSkipOk = false; // 텍스트 스킵 가능 여부
    private bool isResume = false; // 게임 재개 여부
    private Tweener typingTween;  // DOTween 애니메이션 저장 변수
    [SerializeField] private Vector3 cameraZoomInPos;
    [EndTab]

    // 타자 소리 관련 변수들
    [Tab("타자 소리 설정")]
    [SerializeField] private float typeSoundInterval = 0.1f;  // 타자기 소리 간격
    private float timeSinceLastTypeSound = 0f;  // 마지막 타자기 소리가 난 후 경과 시간
    [EndTab]

    // UI 관련 변수들
    [Tab("UI 설정")]
    [SerializeField] private Image fadeImage; // 페이드용 이미지
    [SerializeField] private GameObject pauseScreen; // 일시정지 화면
    [SerializeField] private GameObject skipScreen; // 스킵 화면
    [SerializeField] private GameObject optionScreen; // 옵션 화면

    [SerializeField] private GameObject scriptArrow; // ScriptBar에 위치한 화살표 
    [SerializeField] private Image newsImage; // 뉴스 이미지
    [SerializeField] private Sprite heroImage; // 히어로 이미지
    [SerializeField] private Sprite[] newsImages; // 뉴스 이미지에 사용할 이미지들
    private Selectable pauseScreenFirstSelectButton;

    [EndTab]

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        scriptText.text = "";  // 텍스트 초기화
        StartCoroutine(ScriptBarOnCorutin());

        // 일시정지 화면의 첫번째 선택 버튼을 가져옴
        pauseScreenFirstSelectButton = pauseScreen.GetComponentInChildren<Selectable>(true);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseScreen(!isResume);
        }

        if (isScriptEnd || !isTextSkipOk)
            return;
        
        // 스페이스바를 눌렀을 때
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) 
            && !isResume)
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
    } 

    // 다음 스크립트를 출력하는 함수
    private void DisplayNextScript()
    {
        if (currentScriptIndex > scripts.Length)
            return;

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


        // 뉴스 이미지 설명 하는 장면
        if (currentScriptIndex == 1)
        {
            isTextSkipOk = false;
            scriptArrow.SetActive(false);
            StartCoroutine(ChangeNewsImageCorutin());
        }
        if (currentScriptIndex == 4)
        {
            // 세이프티 히어로 등장
            isTextSkipOk = false;
            scriptArrow.SetActive(false);
            StartCoroutine(ChangeHeroImage());
        }
        if (currentScriptIndex == 8)
        {
            // 카메라가 뉴스 화면으로 점점 줌인이 되는 애니메이션
            isTextSkipOk = false;
            StartCoroutine(ZoomInNewsImage());            
        }
        currentScriptIndex++;  // 다음 텍스트로 이동
    }
    // 스크립트 바가 화면에 나타나는 코루틴
    IEnumerator ScriptBarOnCorutin()
    {
        yield return new WaitForSeconds(1f);
        sciprtBar.DOAnchorPos(Vector3.zero, 0.5f).OnComplete(() =>
        {
            DisplayNextScript();
            isTextSkipOk = true;
        });
    }

    // 히어로 이미지로 변경하는 코루틴
    IEnumerator ChangeHeroImage()
    {
        yield return new WaitForSeconds(1f);
        newsImage.sprite = heroImage;
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
    private IEnumerator ZoomInNewsImage()
    {
        yield return null;
        Transform camerTransform = Camera.main.transform;
        camerTransform.DOLocalMove(cameraZoomInPos, 14f).SetEase(Ease.OutQuart);

        yield return new WaitForSeconds(2f);
        scriptArrow.SetActive(false);
        isTextSkipOk = false;
        Skip();
    }

    // 스킵 버튼 클릭 시
    public void Skip()
    {
        PauseScreen(false);
        fadeImage.GetComponent<Image>().enabled = true;
        fadeImage.DOFade(1, 0.75f).OnComplete(() =>
        {
            LoadingSceneController.LoadScene("Title Scene");
        });
    }
    public void PauseScreen(bool isOpen)
    {        
        Time.timeScale = isOpen ? 0 : 1;        
        isResume = isOpen;
        pauseScreen.SetActive(isOpen);

        if (isOpen)
        {
            pauseScreenFirstSelectButton.Select();
        }
        else
        {
            optionScreen.SetActive(false);
            skipScreen.SetActive(false);
        }
            
        MasterAudio.PlaySound("BtnClick");
    }
    // 스킵 화면 On/Off
    public void SkipScreen(bool isOpen)
    {
        if (isOpen)
        {
            skipScreen.SetActive(true);
            Selectable selectable = skipScreen.GetComponentInChildren<Selectable>();
            selectable.Select();
        }

        else
        {
            skipScreen.SetActive(false);
            pauseScreenFirstSelectButton.Select();
        }
    }
    // 옵션 화면 On/Off
    public void OptionScreen(bool isOpen)
    {
        if (isOpen)
        {
            optionScreen.SetActive(true);
            Selectable selectable = optionScreen.GetComponentInChildren<Selectable>();
            selectable.Select();
        }

        else
        {
            optionScreen.SetActive(false);
            pauseScreenFirstSelectButton.Select();
        }
    }

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}

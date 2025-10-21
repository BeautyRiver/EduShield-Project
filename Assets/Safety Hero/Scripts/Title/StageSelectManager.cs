using DG.Tweening;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectUi; // 캐릭터 선택 창
    [SerializeField] private GameObject stageImageParent; // 스테이지 담고있는 오브젝트
    
    [SerializeField] [VInspector.ReadOnly] private Image[] stageImages; // 스테이지 이미지들
    [SerializeField] [VInspector.ReadOnly] private RectTransform[] stageImageRects;// 스테이지 이미지 rect
    [SerializeField] private int idx = 0;
    [SerializeField] private Vector2[] stageImageScale;
    [SerializeField] private Vector2[] stageImagePos;
    [SerializeField] private Color noneSelectColor;

    private bool isStageSelecting;
    private Vector2 inputVec; // 입력 벡터 (방향)
    private void Awake()
    {
        // 부모 rect는 필터링
        stageImageRects = stageImageParent.GetComponentsInChildren<RectTransform>(true)
                                 .Where(rt => rt != stageImageParent.GetComponent<RectTransform>())
                                 .ToArray();

        stageImages = stageImageParent.GetComponentsInChildren<Image>();        
    }   

    // 다음 버튼
    public void PressNextButton()
    {
        if (idx >= stageImageRects.Length - 1 || DataManager.instance.IsUnlockStages[idx + 1] == false)
            return;

        // 맨 앞에 이미지 왼편으로 치워 버리기
        if (idx > 0)
            MoveStageImage(idx - 1, stageImagePos[0], stageImageScale[0], noneSelectColor);

        // 현재 가운데 이미지 왼쪽으로 한칸 이동
        MoveStageImage(idx, stageImagePos[1], stageImageScale[1], noneSelectColor);

        // 다음 이미지 가운데로 이동
        MoveStageImage(idx + 1, stageImagePos[2], stageImageScale[2], Color.white);

        if (idx + 2 <= stageImageRects.Length - 1)
            MoveStageImage(idx + 2, stageImagePos[3], stageImageScale[1], noneSelectColor);

        // 인덱스 증가
        idx++;
        stageImages[idx].GetComponent<Button>().Select();
    }

    // 이전 버튼
    public void PressPrevButton()
    {
        if (idx <= 0)
            return;
        
        if (idx - 2 >= 0)
            MoveStageImage(idx - 2, stageImagePos[1], stageImageScale[1], noneSelectColor);

        MoveStageImage(idx - 1, stageImagePos[2], stageImageScale[2], Color.white);
        MoveStageImage(idx, stageImagePos[3], stageImageScale[1], noneSelectColor);

        if (idx < stageImageRects.Length - 1)
            MoveStageImage(idx + 1, stageImagePos[4], stageImageScale[0], noneSelectColor);

        // 인덱스 감소
        idx--;
        stageImages[idx].GetComponent<Button>().Select();
    }


    private void MoveStageImage(int index, Vector3 pos, Vector3 scale, Color color)
    {
        if (index < 0 || index >= stageImageRects.Length) return; // 범위 체크
        stageImageRects[index].DOAnchorPos(pos, 0.5f);
        stageImageRects[index].DOScale(scale, 0.5f);
        stageImages[index].DOColor(color, 0.5f);
    }

    // 씬 전환 설정
    public void LoadScene()
    {
        //LoadingSceneController.LoadScene(SceneManager.GetSceneByBuildIndex(idx).name);
        LoadingSceneController.LoadScene("Game Scene");
    }

    public void OnNavigate(InputValue playerInput)
    {
        if (!isStageSelecting)
            return;

        inputVec = playerInput.Get<Vector2>();        

        // 수평 입력 처리
        if (inputVec.x < 0)
        {
            PressPrevButton();            
        }
        else if (inputVec.x > 0)
        {
            PressNextButton();            
        }

        // 수직 입력 처리                
        if (inputVec.y > 0)
        {
            stageImages[idx].GetComponent<Button>().Select();
        }
        
    }

    public void ShowStageSelect(bool isStageSelect)
    {
        isStageSelecting = isStageSelect;
        stageSelectUi.SetActive(isStageSelect);
        if (isStageSelect)
        {
            stageImages[idx].GetComponent<Button>().Select();
        }
    }    
}

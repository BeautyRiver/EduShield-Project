using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{
    private PlayerInputController playerInputController;
    [SerializeField] private TargetScanner scanner;
    [SerializeField] private IInteractable lastNearObj;

    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        if (GlobalManager.instance.playerState == PlayerState.FreeMove)
        {
            UpdateNearestNpc();
        }
        else
        {
            // 상호작용 중일 때는 스캔 중지 및 말풍선 숨기기
            HideLastInteractUi();
        }
    }

    private void UpdateNearestNpc()
    {
        Transform nearstTarget = scanner.nearestTarget;

        if (nearstTarget != null)
        {
            if (lastNearObj == null || (lastNearObj as MonoBehaviour).transform != nearstTarget)
            {
                // (가장 가까운 object가 바뀌었음) 이전 object의 UI 숨기기 
                if (lastNearObj != null)
                {
                    lastNearObj.ShowInteractUi(false);
                }

                lastNearObj = nearstTarget.GetComponent<IInteractable>();
                lastNearObj.ShowInteractUi(true);
            }

            // Npc종류면 플레이어를 바라보게 하기
            if (lastNearObj is Npc npc)
                npc.LookAtPlayer(transform.position);
        }
        else
        {
            if (lastNearObj != null)
            {
                lastNearObj.ShowInteractUi(false);
                lastNearObj = null;
            }
        }
    }

    // 마지막으로 상호작용 UI를 보여주던 오브젝트의 UI 숨기기
    private void HideLastInteractUi()
    {
        if (lastNearObj != null)
        {
            lastNearObj.ShowInteractUi(false);
            lastNearObj = null;
        }
    }

    public void RequestInteraction()
    {
        // 상호작용 시작
        if (lastNearObj != null)
        {
            playerInputController.ChangeState(PlayerState.InUI);
            lastNearObj.Interact(); // 상호작용 시작
        }
        else
        {
            Debug.Log("상호작용할 Npc가 없습니다.");
        }
    }
 
}

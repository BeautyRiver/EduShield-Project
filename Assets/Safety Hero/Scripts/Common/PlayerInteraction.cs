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
        if (playerInputController.currentState == PlayerState.FreeMove)
        {
            UpdateNearestNpc();
        }
        else
        {
            // 상호작용 중일 때는 스캔 중지 및 말풍선 숨기기
            HideLastPrompt();
        }
    }

    private void UpdateNearestNpc()
    {
        Transform nearstTarget = scanner.nearestTarget;

        if (nearstTarget != null)
        {
            if (lastNearObj == null || (lastNearObj as MonoBehaviour).transform != nearstTarget)
            {
                // (가장 가까운 npc가 바뀌었음) 이전 npc의 말풍선 숨기기 
                if (lastNearObj != null)
                {
                    lastNearObj.ShowPrompt(false);
                }

                lastNearObj = nearstTarget.GetComponent<IInteractable>();
                lastNearObj.ShowPrompt(true);
            }
            
            if (lastNearObj is Npc npc)
                npc.LookAtPlayer(transform.position);
        }
        else
        {
            if (lastNearObj != null)
            {
                lastNearObj.ShowPrompt(false);
                lastNearObj = null;
            }
        }
    }

    private void HideLastPrompt()
    {
        if (lastNearObj != null)
        {
            lastNearObj.ShowPrompt(false);
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
            //InteractingCamera();
        }
        else
        {
            Debug.Log("상호작용할 Npc가 없습니다.");
        }
    }
 
}

using UnityEngine;

// NPC 공통사항:
// 상호작용 E 키, E키 UI 
// 항상 플레이어를 바라보게
public abstract class Npc : MonoBehaviour
{    
    private SpriteRenderer spriter;
    private GameObject speechBubble;
    
    private void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        speechBubble = transform.Find("SpeechBubble").gameObject;
        speechBubble.SetActive(false);
    }

    public void LookAtPlayer(Vector3 playerPosition)
    {
        if (playerPosition.x < transform.position.x)
            spriter.flipX = true;
        else
            spriter.flipX = false;
    }

    // 대화 UI 보여주기 
    public void ShowSpeechBubble(bool show)
    {
        // 상호작용 중일 때또는 이미 상태가 같으면 return
        if (LobbyManager.instance.currentState == LobbyState.Interacting || speechBubble.activeSelf == show)
            return;       

        speechBubble.SetActive(show);
    }

    // 상호작용 
    public abstract void Interaction();
}

using UnityEngine;

public class StageSelectNpc : Npc
{
    public override void Interact()
    {
        LobbyUIManager.instance.SwitchStageSelectUi();
        LobbyManager.instance.InteractingCamera();
    }

}

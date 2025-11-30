using UnityEngine;

public class StageSelectNpc : Npc
{
    public override void Interact()
    {
        base.Interact();
        LobbyUIManager.instance.SwitchStageSelectUi();
    }
}

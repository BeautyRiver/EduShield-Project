using UnityEngine;

public class StageSelectNpc : Npc
{
    public override void Interaction()
    {
        LobbyUIManager.instance.SwitchStageSelectUi();
    }
}

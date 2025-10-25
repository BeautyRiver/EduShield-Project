using UnityEngine;

public class StageSelectNpc : Npc
{
    public override void Interaction()
    {
        LobbyManager.instance.OpenMapSelectUI();
    }
}

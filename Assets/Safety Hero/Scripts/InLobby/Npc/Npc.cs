using UnityEngine;

public class Npc : InteractableObject
{
    public void LookAtPlayer(Vector3 playerPosition)
    {
        if (playerPosition.x < transform.position.x)
            spriter.flipX = true;
        else
            spriter.flipX = false;
    }
    public override void Interact()
    {
        LobbyManager.instance.InteractingCamera();
    }

}

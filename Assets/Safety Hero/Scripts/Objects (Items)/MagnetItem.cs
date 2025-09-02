using DarkTonic.MasterAudio;
using UnityEngine;

public class MagnetItem : Item
{
    [SerializeField] private float magnetRange;
    [SerializeField] private float duration;
    protected override void Use()
    {
        ExpScanner expScanner = gm.player.GetComponent<ExpScanner>();
        if (expScanner != null)
            expScanner.ActivateMagnet(magnetRange, duration);

        MasterAudio.PlaySound("Magnet");
        Debug.Log("자석 효과 발동!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Character : MonoBehaviour
{
    public static float Speed { get => GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    public static float WeaponSpeed { get => GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    public static float WeaponRate { get => GameManager.instance.playerId == 1 ? 0.9f : 1f; }
    public static float Damage { get => GameManager.instance.playerId == 2 ? 1.2f : 1f; }
    public static int Count { get => GameManager.instance.playerId == 3 ? 1 : 0; }


}

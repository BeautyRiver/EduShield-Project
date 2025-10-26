using UnityEditor;
using UnityEngine;

public enum StageDifficulty
{
    Easy,
    Medium,
    Hard
}
[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public int stageId;
    public string sceneName;
    public Sprite stageImage;
    public string stageName;
    [TextArea] public string stageDesc;
    public StageDifficulty difficulty;
    public Color difficultyColor;
    public Sprite difficultyImage;
}
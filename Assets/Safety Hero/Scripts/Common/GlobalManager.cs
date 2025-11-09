using UnityEngine;

public enum GlobalSceneState
{
    InLobby,
    InGame
}

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager instance;
    public GlobalSceneState currentState;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}

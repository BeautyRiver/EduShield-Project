using UnityEngine;
using UnityEngine.EventSystems;

public class UIFirstSelctor : MonoBehaviour
{
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}

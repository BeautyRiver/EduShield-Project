using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{    
    protected SpriteRenderer spriter;
    [SerializeField] protected GameObject interactionUI;

    protected virtual void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();        
        interactionUI.SetActive(false);
    }

    // 상호작용 UI 보여주기 
    public virtual void ShowInteractUi(bool show)
    {
        if (interactionUI.activeSelf == show)
            return;       

        interactionUI.SetActive(show);        
    }

    // 상호작용 
    public abstract void Interact();

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipMessage : MonoBehaviour
{
    private TextMeshProUGUI tipText;
    [SerializeField] List<TipMessagesList> tipMessages = new List<TipMessagesList>();
    private void Awake()
    {
        tipText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        int ranTipTitle = Random.Range(0,tipMessages.Count);
        int ranTipMessage = Random.Range(0, tipMessages[ranTipTitle].tipTexts.Length);
        
        tipText.text = tipMessages[ranTipTitle].tipTexts[ranTipMessage];
    }

    [System.Serializable]
    public class TipMessagesList
    {
        public string theme;
        [TextArea]
        public string[] tipTexts;
    }
}

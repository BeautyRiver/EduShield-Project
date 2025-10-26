using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class FadeImage : MonoBehaviour
{
    public float fadeTime = 1f;
    private Image fadeImage;

    private void Awake()
    {       
        fadeImage = GetComponent<Image>();
    }
    private void Start()
    {
        FadeIn();
    }

    private void FadeIn()
    {
        this.gameObject.SetActive(true);
        fadeImage.enabled = true;
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0, fadeTime).OnComplete(() => fadeImage.enabled = false);
    }
}

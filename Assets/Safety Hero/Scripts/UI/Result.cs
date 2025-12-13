using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public Image[] titles;

    private void OnEnable()
    {
        titles = GetComponentsInChildren<Image>(true);
    }
    public void Lose()
    {
        titles[0].DOFade(1, 1f).SetUpdate(true).OnComplete(() =>
        {
            for (int i = 2; i < titles.Length; i++)
            {
                titles[i].gameObject.SetActive(true);
            }
        });
    }

    public void Win()
    {
        titles[1].DOFade(1, 1f).SetUpdate(true).OnComplete(() =>
        {
            for (int i = 2; i < titles.Length; i++)
            {
                titles[i].gameObject.SetActive(true);
            }
        });
    }
}

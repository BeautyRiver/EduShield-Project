using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    private Image progressBar;

    [SerializeField]
    private Image movingImage;

    [SerializeField]
    private float fillSpeed = 0.5f; // 프로그레스 바가 천천히 차오르게 하는 속도 (작을수록 느림)

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }
    
    void Start()
    {
        StartCoroutine(LoadScene());        
    }
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, Time.deltaTime);


                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            // 이미지가 로딩바를 따라 움직이게
            MoveImageAlongProgressBar();
        }
    }

    private void MoveImageAlongProgressBar()
    {
        // 이동할 위치의 범위: -825에서 825까지
        float minPosition = -825f;
        float maxPosition = 825f;

        // fillAmount(0에서 1 사이의 값)를 범위에 맞춰 변환
        float newX = Mathf.Lerp(minPosition, maxPosition, progressBar.fillAmount);

        // 이미지의 위치를 갱신 (x값만 변경)
        Vector2 newPos = movingImage.rectTransform.anchoredPosition;
        newPos.x = newX;

        movingImage.rectTransform.anchoredPosition = newPos;
    }

}

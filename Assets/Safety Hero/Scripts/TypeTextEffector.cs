using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TypeTextEffector : MonoBehaviour
{
    public TextMeshProUGUI scriptText;
    public float textDelay;
    protected IEnumerator TextEffect(string text)
    {
        scriptText.text = string.Empty;
        StringBuilder stringBuilder = new StringBuilder();

        bool insideTag = false; // 태그 안에 있는지 여부를 추적
        for (int i = 0; i < text.Length; i++)
        {
            // 태그의 시작과 끝을 체크
            if (text[i] == '<')
                insideTag = true; // 태그의 시작
            else if (text[i] == '>')
                insideTag = false; // 태그의 끝

            // 태그 안에 있는 경우에는 한 번에 추가
            stringBuilder.Append(text[i]);

            // 태그가 아닌 경우에만 타이핑 딜레이 적용
            if (!insideTag)
            {
                scriptText.text = stringBuilder.ToString();
                yield return new WaitForSeconds(textDelay);
            }
        }

        // 최종적으로 다 출력한 후에도 전체 텍스트를 다시 한 번 적용 (예: 태그가 완전히 출력되지 않았을 경우를 대비)
        scriptText.text = stringBuilder.ToString();
    }
}

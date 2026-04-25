using System.Collections;
using TMPro;
using UnityEngine;

public class TextAreaPopUp : MonoBehaviour
{
    public TMP_Text textArea;

    public static TextAreaPopUp instance;
    private string lastArea;
    private void Awake()
    {
        if (instance != null){
            Destroy (instance.gameObject);
        }

        instance = this;
    }
    private void Start()
    {
        textArea.text = "";
    }
    public void EnterArea(string areaName)
    {
        if (areaName == lastArea && string.IsNullOrEmpty(areaName))
            return;
        StartCoroutine(SwapEnterName());
        textArea.text = areaName;
        lastArea = areaName;
    }

    private IEnumerator SwapEnterName()
    {
        yield return FadeText(1, 0, 1);
        yield return new WaitForSeconds(1);
        yield return FadeText(1, 1, 0);
    }

    private IEnumerator FadeText(float duration, float start, float end)
    {
        Color color = textArea.color;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(start, end, timer / duration);
            textArea.color = color;
            yield return null;
        }

        color.a = end;
        textArea.color = color;
    }
}

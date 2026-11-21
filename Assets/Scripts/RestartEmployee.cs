using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class RestartEmployee : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public UnityEvent ResetNarrator;

    public void FadeToWhite()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        // Fade to white
        yield return StartCoroutine(FadeAlpha(0, 1));

        // Fade back to clear
        yield return StartCoroutine(FadeAlpha(1, 0));

        ResetNarrator.Invoke();
    }

    IEnumerator FadeAlpha(float start, float end)
    {
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            Color colour = fadeImage.color;
            colour.a = Mathf.Lerp(start, end, elapsed / fadeDuration);
            fadeImage.color = colour;

            yield return null;
        }

        Color finalColour = fadeImage.color;
        finalColour.a = end;
        fadeImage.color = finalColour;
    }
}

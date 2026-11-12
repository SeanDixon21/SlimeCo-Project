using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public float speed;
    public Image fadeImage;

    public static SceneChanger instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(SceneFade(Color.clear, ""));
    }

    public void NextScene(string sceneID)
    {
        if (sceneID == "") return;
        StartCoroutine(SceneFade(Color.black, sceneID));
    }

    IEnumerator SceneFade(Color c, string sceneID)
    {
        Color startColour = fadeImage.color;
        float timer = 0;
        while (timer < 1)
        {
            yield return null;
            timer += Time.deltaTime * speed;
            timer = Mathf.Clamp01(timer);
            fadeImage.color = new Color(startColour.r, startColour.b, startColour.g, Mathf.Lerp(startColour.a, c.a, timer));
        }
        if (sceneID != "") SceneManager.LoadSceneAsync(sceneID).completed += OnComplete;
    }

    public void OnComplete(AsyncOperation operation)
    {
        StartCoroutine(SceneFade(Color.clear, ""));
    }

    public void Quit()
    {
        Application.Quit();
    }
}

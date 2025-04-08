using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject loadingScreen;
    public GameObject gameOverScreen;
    public float fadeDuration = 0.6f;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ShowLoadingScreen();
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        StartCoroutine(FadeOutScreen(loadingScreen));
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        CanvasGroup canvasGroup = gameOverScreen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    public void HideGameOverScreen()
    {
        StartCoroutine(FadeOutScreen(gameOverScreen));
    }


    private IEnumerator FadeOutScreen(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        screen.SetActive(false);
    }

}

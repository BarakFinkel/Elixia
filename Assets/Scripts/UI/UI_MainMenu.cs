using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField]
    private string sceneName = "MainScene";

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    private UI_ScreenFade screenFade;

    [SerializeField]
    private float fadeTime = 2.0f;

    public void Start()
    {
        if (!SaveManager.instance.HasSavedData())
        {
            continueButton.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(fadeTime));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSavedData();
        StartCoroutine(LoadSceneWithFadeEffect(fadeTime));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        screenFade.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_EndScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName = "MainMenu";

    [SerializeField]
    private GameObject returnToMainMenuButton;

    [SerializeField]
    private UI_ScreenFade screenFade;

    [SerializeField]
    private float fadeTime = 2.0f;

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadSceneWithFadeEffect(fadeTime));
    }

    private IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        screenFade.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
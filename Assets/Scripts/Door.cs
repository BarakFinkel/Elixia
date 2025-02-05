using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] Sprite openDoor;
    [SerializeField] UI_ScreenFade screenFade;
    [SerializeField] float delay = 2.0f;
    private SpriteRenderer sr;
    private bool canEnter = false;

    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        canEnter = true;
        sr.sprite = openDoor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canEnter && other.CompareTag("Player"))
        {
            SaveManager.instance.SaveGame();
            StartCoroutine(LoadEndGame(delay));
        }
    }

    private IEnumerator LoadEndGame(float _delay)
    {
        screenFade.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene("EndScene");
    }
}
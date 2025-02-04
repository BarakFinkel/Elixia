using UnityEngine;

public class UI_ScreenFade : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeOut()
    {
        anim.SetTrigger("fadeOut");
    }

    public void FadeIn()
    {
        anim.SetTrigger("fadeIn");
    }
}
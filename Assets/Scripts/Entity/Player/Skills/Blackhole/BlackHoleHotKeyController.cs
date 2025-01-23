using TMPro;
using UnityEngine;

public class BlackHoleHotKeyController : MonoBehaviour
{
    private BlackholeSkillController myBlackhole;

    private Transform myEnemy;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;
    private SpriteRenderer sr;

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            myBlackhole.AddEnemyToList(myEnemy);
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }

    public void SetupHotKey(KeyCode _hotKey, Transform _myEnemy, BlackholeSkillController _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        myBlackhole = _myBlackHole;

        myHotKey = _hotKey;
        var text = myHotKey.ToString();
        myText.text = text[text.Length - 1].ToString();
    }
}
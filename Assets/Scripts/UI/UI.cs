using UnityEngine;
using UnityEngine.Serialization;

public class UI : MonoBehaviour
{
    [FormerlySerializedAs("tooltip")]
    public UI_ItemTooltip itemTooltip;

    public UI_StatTooltip statTooltip;

    [SerializeField]
    private GameObject characterUI;

    [SerializeField]
    private GameObject skillsUI;

    [SerializeField]
    private GameObject craftUI;

    [SerializeField]
    private GameObject optionsUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SwitchTo(null);
        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchTo(characterUI);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchTo(skillsUI);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchTo(craftUI);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchTo(optionsUI);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchTo(null);
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            _menu.SetActive(true);
        }
    }

    public void SwitchWithKey(GameObject _menu)
    {
        if (_menu == null || _menu.activeSelf)
        {
            _menu.SetActive(false);
            return;
        }

        SwitchTo(_menu);
    }
}
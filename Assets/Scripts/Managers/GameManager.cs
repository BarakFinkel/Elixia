using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    [SerializeField]
    private Checkpoint[] checkpoints;

    [SerializeField]
    private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField]
    private GameObject lostCurrencyPrefab;

    public int lostCurrencyAmount;

    [SerializeField]
    private float lostCurrencyX;

    [SerializeField]
    private float lostCurrencyY;

    private Transform player;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        player = PlayerManager.instance.player.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            RestartScene();
        }
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data));
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;


        if (FindClosestCheckpoint() != null)
        {
            _data.closestCheckpointId = FindClosestCheckpoint().id;
        }

        _data.checkpoints.Clear();

        foreach (var checkpoint in checkpoints) _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (var pair in _data.checkpoints)
        foreach (var checkpoint in checkpoints)
            if (checkpoint.id == pair.Key && pair.Value)
            {
                checkpoint.ActivateCheckpoint();
            }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            var newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY),
                Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
        {
            return;
        }


        closestCheckpointId = _data.closestCheckpointId;

        foreach (var checkpoint in checkpoints)
            if (closestCheckpointId == checkpoint.id)
            {
                player.position = checkpoint.transform.position;
            }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        var closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            var distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }
}
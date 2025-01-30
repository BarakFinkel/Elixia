using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    private Transform player;

    [SerializeField] private Transform checkpointParent;
    [SerializeField] private List<Checkpoint> checkpoints;
    [SerializeField] private Vector3 playerSpawnOffset;
    [SerializeField] private string closestCheckpointID;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;    
    public int lostCurrencyAmount;


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
        InitCheckpointsList();
        player = PlayerManager.instance.player.transform;
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void InitCheckpointsList()
    {
        for (int i = 0; i < checkpointParent.childCount; i++)
        {
            checkpoints.Add(checkpointParent.GetChild(i).GetComponent<Checkpoint>());
        }
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;
        
        if (FindClosestActiveCheckpoint() != null)
        {
            _data.closestCheckpointID = FindClosestActiveCheckpoint().id;
        }

        _data.checkpoints.Clear();
        
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(0.1f); // Small delay, not meant to change.
        
        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string,bool> pair in _data.checkpoints)
        { 
            foreach (Checkpoint checkpoint in checkpoints)
            {
                // If we find matching id's between the saved data and our current data - 
                // We check in the saved data if the checkpoint is supposed to be active.
                // If so, we activate the checkpoint.
                if (checkpoint.id == pair.Key && pair.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    public void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointID == null)
        {
            return;
        }
        
        closestCheckpointID = _data.closestCheckpointID;
        
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointID == checkpoint.id)
            {
                player.position = checkpoint.transform.position + playerSpawnOffset;
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private Checkpoint FindClosestActiveCheckpoint()
    {
        float closestDistane = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistane && checkpoint.activationStatus == true)
            {
                closestDistane = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }
}

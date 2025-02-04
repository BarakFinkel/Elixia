using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField]
    private string fileName = "ElixiaSave.NikitaBarak";

    [SerializeField]
    private bool encryptData;

    private FileDataHandler dataHandler;
    private readonly string dirPath = "/idbfs/Elixia206332bfnb";
    private GameData gameData;

    [SerializeField]
    private List<ISaveManager> saveManagers;

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
        dataHandler = new FileDataHandler(dirPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();

        //Invoke("LoadGame", .05f);

        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }


    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(dirPath, fileName, encryptData);
        dataHandler.Delete();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }

        foreach (var saveManager in saveManagers) saveManager.LoadData(gameData);
    }

    public void SaveGame()
    {
        foreach (var saveManager in saveManagers) saveManager.SaveData(ref gameData);

        dataHandler.Save(gameData);
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        var saveManagers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }
}
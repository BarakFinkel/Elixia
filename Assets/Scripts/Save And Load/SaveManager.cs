using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    
    [SerializeField] private string fileName;
    [SerializeField] private string filePath = "idbfs/ElixiaGameSaveDirectory206332bfnb";
    [SerializeField] private bool encryptData;
    [SerializeField] private List<ISaveManager> saveManagers;
    public GameData gameData;
    private FileDataHandler dataHandler;

    private bool saveLoaded = false;

    public void Awake()
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

    public void Start()
    {
        dataHandler = new FileDataHandler(filePath, fileName, encryptData);
        FindAllSaveManagers();
        LoadGame();
        saveLoaded = true;
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(filePath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void FindAllSaveManagers()
    {

        IEnumerable<ISaveManager> saveManagersEnum = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveManager>();
        saveManagers = new List<ISaveManager>(saveManagersEnum);
    }

    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }

    public bool LoadStatus()
    {
        return saveLoaded;
    }
}

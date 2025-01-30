using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    
    [SerializeField] private string fileName;
    [SerializeField] private string filePath = "idbfs/ElixiaGameSaveDirectory206332bfnb";
    [SerializeField] private bool encryptData;
    public GameData gameData;
    private FileDataHandler dataHandler;
    private List<ISaveManager> saveManagers;
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
        saveManagers = FindAllSaveManagers();
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

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveManager>();

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

    public bool LoadStatus()
    {
        return saveLoaded;
    }
}

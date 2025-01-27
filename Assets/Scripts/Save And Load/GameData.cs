using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int currency;
    public SerializableDictionary<string,bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public SerializableDictionary<string, bool> checkpoints;
    public List<string> equipmentIDs;
    public string closestCheckpointID;
    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;
        
        this.currency = 0;
        this.skillTree = new SerializableDictionary<string, bool>();
        this.inventory = new SerializableDictionary<string, int>();
        this.checkpoints = new SerializableDictionary<string, bool>();
        this.equipmentIDs = new List<string>();
        this.closestCheckpointID = string.Empty;
    }
}

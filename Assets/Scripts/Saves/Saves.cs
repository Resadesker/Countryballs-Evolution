using UnityEngine;
using System.Collections.Generic;

public class Saves : MonoBehaviour
{
    public static Saves Instance { get; private set; }
    private string tablePrefix = ""; // In case of multi-saves in future. "Save0_" in old saves.
    private List<string> buildingKeys = new List<string>();

    private void Awake()
    {
        Instance = this;
        LoadBuildingKeys();
    }

    // Saves building data
    public void SaveBuilding(string name, int value1, int value2)
    {
        string keyPrefix = $"{tablePrefix}Building_{name}";
        PlayerPrefs.SetInt($"{keyPrefix}_Value1", value1);
        PlayerPrefs.SetInt($"{keyPrefix}_Value2", value2);

        if (!buildingKeys.Contains(name))
        {
            buildingKeys.Add(name);
            SaveBuildingKeys();
        }

        PlayerPrefs.Save();
    }
    
    private void SaveBuildingKeys()
    {
        PlayerPrefs.SetString($"{tablePrefix}BuildingKeys", string.Join(",", buildingKeys));
        PlayerPrefs.Save();
    }
    
    private void LoadBuildingKeys()
    {
        string savedKeys = PlayerPrefs.GetString($"{tablePrefix}BuildingKeys", "");
        if (!string.IsNullOrEmpty(savedKeys))
        {
            buildingKeys = new List<string>(savedKeys.Split(','));
        }
    }

    // Saves extra data
    public void SaveExtra(string name, long value)
    {
        PlayerPrefs.SetString($"{tablePrefix}Extra_{name}_Value", value.ToString());
        PlayerPrefs.Save();
    }

    // Load building's value1 (main data)
    public long LoadBuilding(string name, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt($"{tablePrefix}Building_{name}_Value1", defaultValue);
    }

    // Load building's value2 (level or secondary data)
    public long LoadBuildingLevel(string name, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt($"{tablePrefix}Building_{name}_Value2", defaultValue);
    }

    // Load extra data
    public long LoadExtra(string name, int defaultValue = 0)
    {
        string valueStr = PlayerPrefs.GetString($"{tablePrefix}Extra_{name}_Value", defaultValue.ToString());
        long result;
        if (long.TryParse(valueStr, out result))
        {
            return result;
        }
        return defaultValue;
    }

    // Retrieves buildings of a certain type
    public long GetBuildingsOfType(uint type)
    {
        foreach (string name in buildingKeys)
        {
            string keyPrefix = $"{tablePrefix}Building_{name}";
            if (PlayerPrefs.GetInt($"{keyPrefix}_Value1") == type)
            {
                print("Found building on slot " + PlayerPrefs.GetInt($"{keyPrefix}_Value2"));
                return int.Parse(name);
            }
        }

        return -1; // If no matching building type is found
    }


    // Calculates total working people by summing all jobs (now unused)
    public long GetTotalWorkingPeople()
    {
        return 0; // Since Jobs table is no longer used
    }

    // Deletes all data from PlayerPrefs
    public void DropAllTables()
    {
        PlayerPrefs.DeleteAll();
    }

    public int LoadJob(int j)
    {
        return 0;
    }

    public void SaveJob(int i, int j)
    {
        
    }
}

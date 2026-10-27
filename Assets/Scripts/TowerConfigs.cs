using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfigs", menuName = "Scriptable Objects/TowerConfigs")]
public class TowerConfigs : ScriptableObject
{
    [Serializable]
    public class TowerLevelConfigs
    {
        public int level;
        public GameObject tower;
    }

    [Header("Standert")]
    public string towerName = "Untitled";
    
    [Header("Level configs")]
    [SerializeField] private TowerLevelConfigs[] towerLevelConfigs;

    public Dictionary<int, GameObject> GetTowerLevels()
    {
        Dictionary<int, GameObject> towerLevels = new();

        foreach (TowerLevelConfigs levelConfigs in towerLevelConfigs)
        {
            towerLevels.Add(levelConfigs.level, levelConfigs.tower);
        }

        return towerLevels;
    }
}

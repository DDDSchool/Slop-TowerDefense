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
        public int cost;
        public GameObject tower;
    }

    [Header("Standert")]
    public string towerName = "Untitled";
    public Sprite icon;
    
    [Header("Level configs")]
    [SerializeField] private TowerLevelConfigs[] towerLevelConfigs;
    
    [HideInInspector] public Dictionary<int, TowerLevelConfigs> towerlevels;

    public void ResetTowerLevels()
    {
        towerlevels = new();

        foreach (TowerLevelConfigs levelConfigs in towerLevelConfigs)
        {
            towerlevels.Add(levelConfigs.level, levelConfigs);
        }
    }
}

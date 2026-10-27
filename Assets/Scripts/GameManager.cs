using UnityEditor;
using UnityEngine;

public class GameManager
{
    static TowerConfigs[] towerConfigs = Resources.LoadAll<TowerConfigs>("TowerInfo");
    
    [InitializeOnLoadMethod]
    static void Init()
    {
        foreach (TowerConfigs towerInfo in towerConfigs)
        {
            towerInfo.ResetTowerLevels();
        }
    }
}

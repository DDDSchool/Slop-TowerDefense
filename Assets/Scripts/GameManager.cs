using UnityEditor;
using UnityEngine;

public class GameManager
{
    static TowerConfigs[] towerConfigs = Resources.LoadAll<TowerConfigs>("TowerInfo");
    
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        foreach (TowerConfigs towerInfo in towerConfigs)
        {
            towerInfo.ResetTowerLevels();
        }
    }
}

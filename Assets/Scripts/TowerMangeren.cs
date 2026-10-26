using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TowerMangeren : MonoBehaviour
{
    //classes
    [Serializable]
    private class TowerSelectButtonConfigs
    {
        public Button button;
        public TowerConfigs tower;
        public RawImage image;
    }

    [Header("Canvas Settings")]
    [SerializeField] private TowerSelectButtonConfigs[] towerSelectButtons;

    private List<Tower> towersInGame = new();
    private TowerConfigs[] towerConfigs;
    private Dictionary<TowerConfigs, Dictionary<int, GameObject>> towersLevelTowers = new();
    private bool placingDown = false;
    private TowerConfigs selectedPlacingTowerConfig;
    private Collider selectedPlacingTowerCollider;
    private Tower selectedPlacingTowerScript;
    private Vector3 mouseWorldPPosition;
    private int cantSpawnLayer;

    //configs
    const float mouseRayDis = 10f;
    const float placingDownYPosition = .5f;
    const string towerTag = "Tower";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        towerConfigs = Resources.LoadAll<TowerConfigs>("TowerInfo");
        foreach (TowerConfigs towerConfig in towerConfigs)
        {
            towersLevelTowers.Add(towerConfig, towerConfig.GetTowerLevels());
        }

        //set up the buttons
        foreach (TowerSelectButtonConfigs info in towerSelectButtons)
        {
            info.button.onClick.AddListener(() => SelectPlaceTower(info.tower));
            
            GameObject level1Tower = towersLevelTowers[info.tower][1];
            Texture2D sprite = PrefabUtility.GetIconForGameObject(level1Tower);
            info.image.texture = sprite;
        }

        cantSpawnLayer = LayerMask.GetMask("Can't Fit Tower");
    }

    // Update is called once per frame
    void Update()
    {
        GetMousePosition();

        if (placingDown) PlaceDownTower();
        else TrySelectingTower();
    }

    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        if (!plane.Raycast(ray, out float enter)) return;
        mouseWorldPPosition = ray.GetPoint(enter);
    }

    #region Placing/Moving Towers
    void PlaceDownTower()
    {   
        Vector3 targetPosition = MakeIt2DPosition(mouseWorldPPosition);
        selectedPlacingTowerScript.transform.position = targetPosition;
        //look or there is something in the way
        Collider[] hits = Physics.OverlapBox(targetPosition, selectedPlacingTowerCollider.bounds.size, Quaternion.identity, cantSpawnLayer);

        if (hits.Length != 0)
        {
            print("make it red");
            return;
        }

        if (!Input.GetMouseButtonDown(0)) return;

        placingDown = false;
        selectedPlacingTowerScript.enabled = true;
        selectedPlacingTowerScript = null;
        selectedPlacingTowerCollider.enabled = true;

        Vector3 MakeIt2DPosition(Vector3 target)
        {
            return new Vector3(target.x, placingDownYPosition, target.z);
        }
    }

    void SelectPlaceTower(TowerConfigs tower)
    {
        if (selectedPlacingTowerScript != null)
        {
            Destroy(selectedPlacingTowerScript.gameObject);
            selectedPlacingTowerScript = null;
        }
        if (placingDown && tower == selectedPlacingTowerScript) {placingDown = false; return;}

        placingDown = true;
        selectedPlacingTowerConfig = tower;
        selectedPlacingTowerScript = Instantiate(towersLevelTowers[tower][1], transform).GetComponent<Tower>();
        selectedPlacingTowerCollider = selectedPlacingTowerScript.GetComponent<Collider>();
    }
    #endregion

    #region Upgrade/setting of tower
    void TrySelectingTower()
    {
        bool succes = Physics.Raycast(mouseWorldPPosition + Vector3.up * 2, Vector3.down, out RaycastHit hit, mouseRayDis, cantSpawnLayer);
        if (!succes || !hit.transform.CompareTag(towerTag) || !Input.GetMouseButtonDown(0)) return;

        print($"selected {hit.transform.gameObject}");
    }

    void SelectTowerUpgrade()
    {
        
    }
    #endregion
}

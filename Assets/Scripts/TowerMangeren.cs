using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerMangeren : MonoBehaviour
{
    //classes
    [Serializable]
    private class TowerSelectButtonConfigs
    {
        public Button button;
        public TowerConfigs tower;
        public TextMeshProUGUI cost;
    }

    [Serializable]
    private class TowerUpgradeGui
    {
        public GameObject frame;
        public Image showcase;
        public TextMeshProUGUI name;
        public TextMeshProUGUI stats;
        public TextMeshProUGUI buttonText;
        public Button upgradeButton;
    }

    //enums
    private enum SelectState
    {
        none,
        placing,
        upgrade
    }

    [Header("Canvas Settings")]
    [SerializeField] private TowerSelectButtonConfigs[] towerSelectButtons;
    [SerializeField] private TowerUpgradeGui upgradeGui;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("other...")]
    [SerializeField] private GameObject rangeRadiusExample;
    [SerializeField] private GameObject selectRadiusExample;

    private List<Tower> towersInGame = new();
    private TowerConfigs[] towerConfigs;
    private Vector3 mouseWorldPPosition;
    private int cantSpawnLayer;
    private SpriteRenderer rangeRadiusObject;
    private SpriteRenderer selectRadiusObject;
    private SelectState selectState = SelectState.none;

    private TowerConfigs selectedPlacingTowerConfig;
    private Collider selectedPlacingTowerCollider;
    private Tower selectedPlacingTowerScript;

    private GameObject selectedUpgradeTower = null;

    //configs
    const float mouseRayDis = 10f;
    const float placingDownYPosition = .5f;
    const float overlapAlpha = .4f;
    const string towerTag = "Tower";
    const int maxTowerLevel = 6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set up the buttons
        foreach (TowerSelectButtonConfigs info in towerSelectButtons)
        {
            info.button.onClick.AddListener(() => SelectPlaceTower(info.tower));
            
            GameObject level1Tower = info.tower.towerlevels[1].tower;
            info.cost.text = $"{info.tower.towerlevels[1].cost}";
        }

        cantSpawnLayer = LayerMask.GetMask("Can't Fit Tower");
        rangeRadiusObject = Instantiate(rangeRadiusExample, transform).GetComponent<SpriteRenderer>();
        selectRadiusObject = Instantiate(selectRadiusExample, transform).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMousePosition();

        if (selectState == SelectState.placing) PlaceDownTower();
        else TrySelectingTower();
    }

    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        if (!plane.Raycast(ray, out float enter)) return;
        mouseWorldPPosition = ray.GetPoint(enter);
    }

    //hardcoded ye
    public void ToggleSpeed()
    {
        int newTimeScale = Time.timeScale == 1? 2 : 1;
        Time.timeScale = newTimeScale;
        speedText.text = $"{newTimeScale}x";
    }

    #region Placing/Moving Towers
    void PlaceDownTower()
    {   
        Vector3 targetPosition = MakeIt2DPosition(mouseWorldPPosition);
        selectedPlacingTowerScript.transform.position = targetPosition;
        rangeRadiusObject.transform.position = targetPosition;
        //look or there is something in the way
        Collider[] hits = Physics.OverlapBox(targetPosition, selectedPlacingTowerCollider.bounds.size, Quaternion.identity, cantSpawnLayer);

        if (hits.Length != 0)
        {
            print("make it red");
            Color overlapColor = Color.red;
            overlapColor.a = overlapAlpha;
            rangeRadiusObject.color = overlapColor;
            return;
        }

        Color color = Color.black;
        color.a = overlapAlpha;
        rangeRadiusObject.color = color;
        if (!OnMouseWorldClick()) return;
        
        selectState = SelectState.none;
        InGameManager.instance.AddCurrency(-selectedPlacingTowerScript.towerConfigs.towerlevels[1].cost);
        selectedPlacingTowerScript.enabled = true;
        selectedPlacingTowerScript = null;
        selectedPlacingTowerCollider.enabled = true;
        rangeRadiusObject.enabled = false;

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
        if (selectState == SelectState.placing && tower == selectedPlacingTowerScript) {selectState = SelectState.none; return;}
        
        int cost = tower.towerlevels[1].cost;
        if (InGameManager.instance.currency < cost) return;

        selectState = SelectState.placing;
        selectedPlacingTowerConfig = tower;
        selectedPlacingTowerScript = Instantiate(tower.towerlevels[1].tower, transform).GetComponent<Tower>();
        selectedPlacingTowerScript.towerConfigs = tower;
        selectedPlacingTowerCollider = selectedPlacingTowerScript.GetComponent<Collider>();
        rangeRadiusObject.transform.localScale = Vector3.one * selectedPlacingTowerScript.range * 2;
        rangeRadiusObject.enabled = true;
    }
    #endregion

    #region Upgrade/setting of tower
    void TrySelectingTower()
    {
        bool succes = Physics.Raycast(mouseWorldPPosition + Vector3.up * 2, Vector3.down, out RaycastHit hit, mouseRayDis, cantSpawnLayer);
        bool haveClick = OnMouseWorldClick();
        if (!succes || !hit.transform.CompareTag(towerTag))
        {
            selectRadiusObject.enabled = false;
            if (haveClick) DeselectTowerUpgrade();
            return;
        }
        
        selectRadiusObject.transform.position = hit.transform.position;
        selectRadiusObject.enabled = true;

        if (!haveClick) return;

        if (selectedUpgradeTower != hit.transform.gameObject) SelectTowerUpgrade(hit.transform.GetComponent<Tower>());
        else DeselectTowerUpgrade();
    }

    void SelectTowerUpgrade(Tower tower)
    {
        selectState = SelectState.upgrade;
        selectedUpgradeTower = tower.gameObject;
        TowerConfigs configs = tower.towerConfigs;

        upgradeGui.stats.text = tower.GetStatsInfo();
        upgradeGui.name.text = configs.towerName;
        upgradeGui.showcase.sprite = configs.icon;
        upgradeGui.frame.SetActive(true);

        //get next level value for showcase
        upgradeGui.upgradeButton.onClick.RemoveAllListeners();
        
        if (tower.level != maxTowerLevel)
        {
            upgradeGui.buttonText.text = $"Upgrade: cost{configs.towerlevels[tower.level + 1].cost}";
            upgradeGui.upgradeButton.onClick.AddListener(() => UpgradeTower(tower));
        }
        else upgradeGui.buttonText.text = "Maxed Upgraded";

        rangeRadiusObject.transform.localScale = Vector3.one * tower.range * 2;
        rangeRadiusObject.transform.position = tower.transform.position;
        rangeRadiusObject.enabled = true;
    }

    void DeselectTowerUpgrade()
    {
        selectState = SelectState.none;
        selectedUpgradeTower = null;
        upgradeGui.frame.SetActive(false);
        rangeRadiusObject.enabled = false;
    }

    void UpgradeTower(Tower tower)
    {
        TowerConfigs configs = tower.towerConfigs;
        int cost = configs.towerlevels[tower.level + 1].cost;
        if (InGameManager.instance.currency < cost) return;

        InGameManager.instance.AddCurrency(-cost);
        GameObject newObject = configs.towerlevels[tower.level + 1].tower;
        Vector3 spawnPosition = tower.transform.position;
        Destroy(tower.gameObject);

        newObject = Instantiate(newObject, spawnPosition, Quaternion.identity, transform);
        selectedUpgradeTower = newObject;
        newObject.GetComponent<Collider>().enabled = true;
        Tower newTower = newObject.GetComponent<Tower>();
        newTower.towerConfigs = configs;
        newTower.enabled = true;

        SelectTowerUpgrade(newTower);
    }
    #endregion

    #region helper function
    bool OnMouseWorldClick()
    {
        return (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject());
    }
    #endregion
}

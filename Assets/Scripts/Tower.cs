using DG.Tweening;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public enum FightMode
    {
        closest,
        farthest,
        first,
        last,
        random
    }

    [Header("Settings")]
    public int level;
    public int damage;
    public float range;
    public float attackSpeed;
    public float projectileSpeed = 20;
    public int Piercing = 1;

    //visuale
    [SerializeField] protected Transform shootPivit;
    [SerializeField] protected Transform towerRotation;
    [SerializeField] protected GameObject projectile;

    [HideInInspector] public FightMode fightMode = FightMode.random;
    [HideInInspector] public TowerConfigs towerConfigs;

    protected float attackCurrentCooldown = 0;
    protected Vector3 enemieCheckPosition;
    protected quaternion normalTowerRotation;
    
    //configs
    protected const float enemieCheckYPos = 1;
    protected int enemieLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        enemieCheckPosition = transform.position;
        enemieCheckPosition.y = enemieCheckYPos;
        normalTowerRotation = towerRotation.rotation;

        enemieLayer = LayerMask.GetMask("enemy");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (CanShoot()) Shoot();
    }

    //check or it can attack
    protected virtual bool CanShoot()
    {
        float timeBetweenShot = 1 / attackSpeed;
        if (timeBetweenShot < attackCurrentCooldown) return true;

        attackCurrentCooldown += Time.deltaTime;
        return false;
    }

    protected virtual void Shoot()
    {
        Collider[] hits = Physics.OverlapSphere(enemieCheckPosition, range, enemieLayer);
        
        if (hits.Length == 0) return;
        attackCurrentCooldown = 0;

        if (fightMode == FightMode.random)
        {
            int randomEnemie = UnityEngine.Random.Range(0, hits.Length);
            Transform enemyTarget = hits[randomEnemie].transform;
            FireProjectile(enemyTarget);
        }
    }

    protected virtual void FireProjectile(Transform enemyTarget)
    {
        Vector3 spawnPosition = new Vector3(shootPivit.position.x, 1, shootPivit.position.z);
        Vector3 targetPosition = new Vector3(enemyTarget.position.x, 1, enemyTarget.position.z);
        targetPosition = (targetPosition - spawnPosition).normalized * range + spawnPosition;
        towerRotation.rotation = Quaternion.LookRotation(targetPosition - towerRotation.position) * normalTowerRotation;

        GameObject newProjectile = Instantiate(projectile, spawnPosition, towerRotation.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        
        projectileScript.tower = this;
        projectileScript.life = Piercing;
        float duration = (spawnPosition - targetPosition).magnitude / projectileSpeed;
        newProjectile.transform.DOMove(targetPosition, duration)
         .SetAutoKill(false)
         .OnComplete(() => Destroy(newProjectile));
    }

    public virtual void OnHit(Enemy enemy)
    {
        enemy.health -= damage;
    }

    public virtual string GetStatsInfo()
    {
        bool succes = towerConfigs.towerlevels.TryGetValue(level + 1, out TowerConfigs.TowerLevelConfigs levelConfigs); //for now

        if (succes)
        {
            Tower newTower = levelConfigs.tower.GetComponent<Tower>(); //for now;
            string statsInfo = $"Damage: {damage} > {newTower.damage}\n Range: {range} > {newTower.range}\n AttackSpeed: {attackSpeed} > {newTower.attackSpeed}";
            return statsInfo;
        }

        string maxStatsInfo = $"Damage: {damage} > {damage}\n Range: {range} > {range}\n AttackSpeed: {attackSpeed} > {attackSpeed}";
        return maxStatsInfo;
    }

    protected void OnDrawGizmos()
    {
        if (range == 0) return;

        Color hitboxColor = Color.red;
        hitboxColor.a = .5f;
        Handles.color = hitboxColor;
        Handles.DrawSolidDisc(new Vector3(transform.position.x, enemieCheckYPos, transform.position.z), Vector3.up, range);
    }
}

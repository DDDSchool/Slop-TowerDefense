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
    [SerializeField] protected GameObject hitVfx;
    [SerializeField] protected AudioSource sfxPlayer;
    [SerializeField] protected AudioClip hitSfx;

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

        int randomEnemie = UnityEngine.Random.Range(0, hits.Length);

        //get the closest enemie to goal
        Transform enemyTarget = hits[0].transform; //default
        float closet = float.MinValue;

        foreach (Collider hit in hits)
        {
            float dis = hit.GetComponent<Enemy>().DistanceLeft();

            if (closet < dis)
            {
                closet = dis;
                enemyTarget = hit.transform;
            }
        }

        FireProjectile(enemyTarget);
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

        GameObject vfxObject = Instantiate(hitVfx, enemy.transform);
        vfxObject.transform.localPosition = Vector3.zero;
        Destroy(vfxObject, .6f); //fast

        sfxPlayer.PlayOneShot(hitSfx);
    }

    public virtual string GetStatsInfo()
    {
        bool succes = towerConfigs.towerlevels.TryGetValue(level + 1, out TowerConfigs.TowerLevelConfigs levelConfigs); //for now

        if (succes)
        {
            Tower newTower = levelConfigs.tower.GetComponent<Tower>(); //for now;
            string statsInfo = $"Damage: {damage} > {newTower.damage}\n Range: {range} > {newTower.range}\n AttackSpeed: {attackSpeed} > {newTower.attackSpeed}\n Pierces: {Piercing} > {newTower.Piercing}";
            return statsInfo;
        }

        string maxStatsInfo = $"Damage: {damage} > {damage}\n Range: {range} > {range}\n AttackSpeed: {attackSpeed} > {attackSpeed}\n Pierces: {Piercing} > {Piercing}";
        return maxStatsInfo;
    }

    #if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        if (range == 0) return;

        Color hitboxColor = Color.red;
        hitboxColor.a = .5f;
        Handles.color = hitboxColor;
        Handles.DrawSolidDisc(new Vector3(transform.position.x, enemieCheckYPos, transform.position.z), Vector3.up, range);
    }
    #endif
}

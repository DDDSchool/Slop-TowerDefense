using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public enum FightMode
    {
        closest,
        random
    }

    [Header("Settings")]
    public int level;
    public int damage;
    public float range;
    public float attackSpeed;

    //visuale
    [SerializeField] protected Transform shootPivit;

    [HideInInspector] public FightMode fightMode = FightMode.random;

    protected float attackCurrentCooldown = 0;
    protected Vector3 enemieCheckPosition;
    
    //configs
    protected const float enemieCheckYPos = 1;
    protected int enemieLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        enemieCheckPosition = transform.position;
        enemieCheckPosition.y = enemieCheckYPos;

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
            int randomEnemie = Random.Range(0, hits.Length);
            Enemy enemyScript = hits[randomEnemie].GetComponent<Enemy>();
            OnHit(enemyScript);
        }
    }

    protected virtual void OnHit(Enemy enemy)
    {
        enemy.health -= damage;
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

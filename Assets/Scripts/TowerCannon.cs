using UnityEngine;

public class TowerCannon : Tower
{
    public float aoeRadius = 2;

    public override void OnHit(Enemy enemy)
    {
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, aoeRadius, enemieLayer);

        foreach (Collider collider in hits)
        {
            Enemy hit = collider.GetComponent<Enemy>();
            hit.health -= damage;
        }
    }
}

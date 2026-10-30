using UnityEngine;

public class TowerCannon : Tower
{
    public float aoeRadius = 2;

    [SerializeField] private GameObject explosionVfx;

    const float explosionDur = .2f;

    public override void OnHit(Enemy enemy)
    {
        //sfx
        sfxPlayer.PlayOneShot(hitSfx);

        //vfx
        GameObject newVfx = Instantiate(explosionVfx, enemy.transform.position, Quaternion.identity);
        newVfx.transform.localScale = Vector3.one * aoeRadius;
        Destroy(newVfx, explosionDur);

        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, aoeRadius, enemieLayer);

        foreach (Collider collider in hits)
        {
            Enemy hit = collider.GetComponent<Enemy>();
            hit.health -= damage;

            GameObject vfxObject = Instantiate(hitVfx, collider.transform);
            vfxObject.transform.localPosition = Vector3.zero;
            Destroy(vfxObject, .6f); //fast
        }
    }

    public override string GetStatsInfo()
    {
        bool succes = towerConfigs.towerlevels.TryGetValue(level + 1, out TowerConfigs.TowerLevelConfigs levelConfigs); //for now

        if (succes)
        {
            TowerCannon newTower = levelConfigs.tower.GetComponent<TowerCannon>(); //for now;
            string statsInfo = $"Damage: {damage} > {newTower.damage}\n Range: {range} > {newTower.range}\n AttackSpeed: {attackSpeed} > {newTower.attackSpeed}\n Aeo: {aoeRadius} > {newTower.aoeRadius}";
            return statsInfo;
        }

        string maxStatsInfo = $"Damage: {damage} > {damage}\n Range: {range} > {range}\n AttackSpeed: {attackSpeed} > {attackSpeed}\n Aeo: {aoeRadius} > {aoeRadius}";
        return maxStatsInfo;
    }
}

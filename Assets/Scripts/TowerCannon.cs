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
}

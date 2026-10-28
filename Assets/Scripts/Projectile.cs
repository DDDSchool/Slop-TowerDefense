using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Tower tower;
    [HideInInspector] public int life;

    void OnTriggerEnter(Collider other)
    {
        if (tower == null || !other.CompareTag("Enemy")) return;

        life -= 1;
        tower.OnHit(other.GetComponent<Enemy>());

        if (life == 0)
        {
            Destroy(gameObject);
        }
    }
}

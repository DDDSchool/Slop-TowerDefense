using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speed = 4f;
    public int reward = 10;
    private int index = 0;
    public int health = 10;

    void Start()
    {
        StartCoroutine(WaitUntilDead());
    }

    void Update() 
    {
        Transform target = Waypoints.points[index];
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.2f) 
        {
            index++;
            if (index >= Waypoints.points.Length) { ReachedEnd(); }
        }
    }

    IEnumerator WaitUntilDead()
    {
        yield return new WaitUntil(() => health <= 0);

        InGameManager.instance.AddCurrency(reward);
        Destroy(gameObject);
    }

    void ReachedEnd() 
    {
        InGameManager.instance.DamageTower(health);
        Destroy(gameObject);
    }
}
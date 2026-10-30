using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speed = 4f;
    public int reward = 10;
    private int index = 0;
    public int health = 10;

    const float enemieIndexDis = 20;

    void Start()
    {
        StartCoroutine(WaitUntilDead());
    }

    void Update() 
    {
        Transform target = Waypoints.points[index];
        Vector3 dir = target.position - transform.position;
        transform.rotation = quaternion.LookRotation(dir, Vector3.up);
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

    //rq index = 20
    public float DistanceLeft()
    {
        Transform target = Waypoints.points[index];
        float distance = -(target.position - transform.position).magnitude;
        distance += enemieIndexDis * index;

        return distance;
    }
}
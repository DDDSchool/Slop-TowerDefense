using UnityEngine;

public class Waypoints : MonoBehaviour {
    public static Transform[] points;

#if UNITY_EDITOR
    private LineRenderer playtestLine;
#endif

    void Awake() 
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++) points[i] = transform.GetChild(i);


    }
    
    #if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i++) {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
    #endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [SerializeField] List<GameObject> patrolPoints;
    public List<GameObject> PatrolPoints => patrolPoints;
    
    public GameObject GetNextPatrolPoint(GameObject lastPoint)
    {
        int index = patrolPoints.IndexOf(lastPoint);
        Debug.Log("index: " + index);
        if (index == patrolPoints.Count - 1)
            return patrolPoints[0];
        return patrolPoints[index + 1];
    }
}

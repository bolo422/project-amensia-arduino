using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    private List<GameObject> patrolPoints;
    public List<GameObject> PatrolPoints => patrolPoints;
    
    void Awake()
    {
        // look for children of this transform and add to the list
        patrolPoints = new List<GameObject>();
        foreach (Transform child in transform)
        {
            patrolPoints.Add(child.gameObject);
        }
        
        // sor the patrol points by name
        patrolPoints.Sort((x, y) => 
            GetNumberFromString(x.name).CompareTo(GetNumberFromString(y.name)));
    }
    
    private static int GetNumberFromString(string str)
    {
        string numberString = Regex.Match(str, @"\d+").Value;
        return int.Parse(numberString);
    }
    
    public GameObject GetNextPatrolPoint(GameObject lastPoint)
    {
        int index = patrolPoints.IndexOf(lastPoint);
        // Debug.Log("index: " + index);
        if (index == patrolPoints.Count - 1)
            return patrolPoints[0];
        return patrolPoints[index + 1];
    }
}

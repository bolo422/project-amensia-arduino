using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // make so the monster throw a raycast in front of him, trying to detect the player
    // the monster should throw 3 raycasts, one in front, one in front +30 degrees and one in front -30 degrees

    private float visionDistance = 2f;
    private string playerTag = "PlayerInnerCircle";
    private int wallsLayerMask;
    private int detectionCircleLayerMask;
    private int playerLayerMask;
    private bool playerDetected = false;
    private Vector3 lastPlayerPosition;
    private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private GameObject aiDestination;
    private List<GameObject> patrolPoints;
    private bool playerNotFoundCoroutineRunning = false;
    private bool inRoute = true;
    private GameObject lastPatrolPoint;

    private void Awake()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    private void Start()
    {
        //aiDestination.transform.position = transform.position;
        wallsLayerMask = LayerMask.GetMask("Walls");
        detectionCircleLayerMask = LayerMask.GetMask("DetectionCircle");
        playerLayerMask = LayerMask.GetMask("PlayerLayer");
        // find all PatrolRoute objects and get their patrolPoints
        patrolPoints = new List<GameObject>();
        var patrolRoutes = FindObjectsOfType<PatrolRoute>();
        foreach (var patrolRoute in patrolRoutes)
        {
            patrolPoints.AddRange(patrolRoute.PatrolPoints);
        }
    }

    private void FixedUpdate()
    {
        var up = transform.up;
        var position = transform.position;
        var hit1 = Physics2D.Raycast(position, up, visionDistance, detectionCircleLayerMask);
        var hit2 = Physics2D.Raycast(position, Quaternion.Euler(0, 0, 30) * up, visionDistance, detectionCircleLayerMask);
        var hit3 = Physics2D.Raycast(position, Quaternion.Euler(0, 0, -30) * up, visionDistance, detectionCircleLayerMask);
        Debug.DrawRay(transform.position, up * visionDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, 30) * up * visionDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -30) * up * visionDistance, Color.red);
        
        // check if it's colliding with PlayerInnerCircle tag
        if(hit1.collider != null && hit1.collider.CompareTag(playerTag)
           || hit2.collider != null && hit2.collider.CompareTag(playerTag)
           || hit3.collider != null && hit3.collider.CompareTag(playerTag))
        {
            if (IsPlayerVisible())
            {
                //Debug.Log("Player detected");
                playerDetected = true;
                lastPlayerPosition = PlayerLamp.Instance.transform.position;
                aiDestination.transform.position = new Vector3(lastPlayerPosition.x, lastPlayerPosition.y, lastPlayerPosition.z);
                inRoute = false;
            }
        }
        else
        {
            playerDetected = false;
        }

        if (transform.position == aiDestination.transform.position && !playerNotFoundCoroutineRunning &&
            !playerDetected)
            StartCoroutine(PlayerNotFoundCoroutine());

        if (inRoute)
        {
            // check if position is < 0.1f from lastPatrolPoint, if it is, get from parent the PatrolRoute component and then get the next patrol point
            if (Vector3.Distance(transform.position, aiDestination.transform.position) <= 0.2f)
            {
                if (lastPatrolPoint == null)
                    lastPatrolPoint = FindClosestPatrolPoint();
                var patrolRoute = lastPatrolPoint.transform.parent.GetComponent<PatrolRoute>();
                var nextPatrolPoint = patrolRoute.GetNextPatrolPoint(lastPatrolPoint);
                aiDestination.transform.position = nextPatrolPoint.transform.position;
                lastPatrolPoint = nextPatrolPoint;
            }
        }
    }

    private bool IsPlayerVisible()
    {
        var playerPosition = PlayerLamp.Instance.transform.position;
        var hits = Physics2D.RaycastAll(transform.position, playerPosition - transform.position, 10f, playerLayerMask | wallsLayerMask);
        // Debug.DrawRay(playerPosition, transform.position - playerPosition, Color.red);
        bool hitPlayer = false;
        foreach (RaycastHit2D hit2 in hits)
        {
            if (hit2.collider.CompareTag("Player"))
            {
                hitPlayer = true;
                break;
            }
            if (hit2.collider.CompareTag("Wall"))
                break;
        }
        return hitPlayer;
    }
    
    IEnumerator PlayerNotFoundCoroutine()
    {
        playerNotFoundCoroutineRunning = true;
        yield return new WaitForSeconds(3f);
        //find closest patrol point and set it as destination
        var closestPatrolPoint = FindClosestPatrolPoint();
        aiDestination.transform.position = closestPatrolPoint.transform.position;
        playerNotFoundCoroutineRunning = false;
        inRoute = true;
        lastPatrolPoint = closestPatrolPoint;
    }

    private GameObject FindClosestPatrolPoint()
    {
        var closestPatrolPoint = patrolPoints[0];
        var closestDistance = Vector3.Distance(transform.position, closestPatrolPoint.transform.position);
        foreach (var patrolPoint in patrolPoints)
        {
            var distance = Vector3.Distance(transform.position, patrolPoint.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPatrolPoint = patrolPoint;
            }
        }
        return closestPatrolPoint;
    }
}



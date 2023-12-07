using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : MonoBehaviour
{
    [SerializeField] WayPoints wayPoints;
    [SerializeField] int speed;
    Transform currentWayPoint;

    private void Start()
    {
        currentWayPoint = wayPoints.transform.GetChild(0);
        transform.position = currentWayPoint.position;
        currentWayPoint = wayPoints.nextPoint(currentWayPoint);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWayPoint.position) < 0.1f && currentWayPoint.GetSiblingIndex() < wayPoints.transform.childCount - 1)
        {
            currentWayPoint = wayPoints.nextPoint(currentWayPoint);
        }
    }
}

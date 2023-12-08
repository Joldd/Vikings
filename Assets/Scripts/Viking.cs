using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : MonoBehaviour
{
    WayPoints myWayPoints;
    [SerializeField] int speed;
    GameObject currentMark;

    private void Start()
    {
        myWayPoints = GameManager.Instance.currentWayPoints;
        currentMark = myWayPoints.marks[0];
        transform.position = currentMark.transform.position;
        currentMark = myWayPoints.nextPoint(currentMark);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentMark.transform.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
        {
            currentMark = myWayPoints.nextPoint(currentMark);
        }
    }
}

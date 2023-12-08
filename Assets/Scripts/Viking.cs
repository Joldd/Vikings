using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : MonoBehaviour
{
    WayPoints myWayPoints;
    [SerializeField] int speed;
    GameObject currentMark;
    LineRenderer currentLine;

    private void Start()
    {
        myWayPoints = GameManager.Instance.currentWayPoints;
        currentMark = myWayPoints.marks[0];
        transform.position = currentMark.transform.position;
        currentMark.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
    }

    private void Update()
    {
        if (myWayPoints != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentMark.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
            {
                currentMark.SetActive(false);
                currentMark = myWayPoints.nextPoint(currentMark);
                currentLine.gameObject.SetActive(false);
                currentLine = myWayPoints.nextLine(currentLine);
            }
            currentLine.SetPosition(0, transform.position);
            if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) == myWayPoints.marks.Count - 1)
            {
                Destroy(myWayPoints.gameObject);
            }
        }    
    }
}

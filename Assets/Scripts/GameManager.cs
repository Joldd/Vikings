using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] WayPoints wayPoints;
    public WayPoints currentWayPoints;
    [SerializeField] GameObject viking;
    [SerializeField] GameObject mark;
    List<GameObject> marks = new List<GameObject>();
    GameObject currentMark;
    bool isPathing;
    [SerializeField] GameObject floor;
    [SerializeField] LineRenderer _lr;
    LineRenderer currentLine;
    List<LineRenderer> lines = new List<LineRenderer>();
    [SerializeField] float lineWidth = 0.2f;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void createPath()
    {
        if (!isPathing)
        {
            isPathing = true;
            currentWayPoints = Instantiate(wayPoints);
            currentMark = Instantiate(mark, currentWayPoints.transform);
            marks.Add(currentMark);
        }   
    }

    public void createViking()
    {
        if (GameObject.FindObjectOfType<WayPoints>() != null && !isPathing)
        {
            Instantiate(viking);
        }
    }

    private void FixedUpdate()
    {
        if (isPathing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                currentMark.transform.position = hit.point;
                if (marks.Count >= 2)
                {
                    currentLine.SetPosition(1, hit.point);
                }
            }
        }
        
    }

    private void Update()
    {
        if (isPathing && Input.GetMouseButtonDown(0))
        {
            Vector3 currentPos = currentMark.transform.position;
            currentMark = Instantiate(mark, currentWayPoints.transform);
            marks.Add(currentMark);
            currentMark.transform.position = currentPos;
            if (marks.Count >= 2)
            {
                int index = marks.IndexOf(currentMark);
                currentLine = Instantiate(_lr, currentWayPoints.transform);
                currentLine.SetPosition(0, marks[index - 1].transform.position);
                currentLine.SetPosition(1, currentMark.transform.position);
                currentLine.startWidth = lineWidth;
                currentLine.endWidth = lineWidth;
                lines.Add(currentLine);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            marks.Remove(currentMark);
            DestroyImmediate(currentMark);
            DestroyImmediate(currentLine);
            currentWayPoints.setMarks(marks);
            marks.Clear();
            currentWayPoints.setLines(lines);
            lines.Clear();
            isPathing = false;
        }
    }
}

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
                
            }
        }
        
    }

    private void Update()
    {
        if (isPathing && Input.GetMouseButtonDown(0))
        {
            currentMark = Instantiate(mark, currentWayPoints.transform);
            marks.Add(currentMark);
            if (marks.Count >= 3)
            {
                int index = marks.IndexOf(currentMark);
                LineRenderer line = Instantiate(_lr, currentWayPoints.transform);
                line.SetPosition(0, marks[index - 2].transform.position);
                line.SetPosition(1, marks[index - 1].transform.position);
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            marks.Remove(currentMark);
            DestroyImmediate(currentMark);
            currentWayPoints.setMarks(marks);
            marks.Clear();
            isPathing = false;
        }
    }
}

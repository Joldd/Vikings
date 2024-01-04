using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] WayPoints wayPoints;
    public WayPoints currentWayPoints;

    [SerializeField] GameObject mark;
    List<GameObject> marks = new List<GameObject>();
    GameObject currentMark;

    bool isPathing;

    [SerializeField] LineRenderer _lr;
    LineRenderer currentLine;
    List<LineRenderer> lines = new List<LineRenderer>();
    [SerializeField] float lineWidth = 0.2f;

    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject floor;
    int layer_mask;

    public Selectable selectedUnit;

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

    private void Start()
    {
       layer_mask = LayerMask.GetMask("Floor");
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

    public void createViking(GameObject viking, Transform spawn)
    {
        GameObject v = Instantiate(viking);
        v.transform.position = spawn.transform.position;
    }

    private void FixedUpdate()
    {
        if (isPathing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layer_mask))
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

        if (isPathing && Input.GetKeyDown(KeyCode.Escape))
        {
            marks.Remove(currentMark);
            DestroyImmediate(currentMark);
            DestroyImmediate(currentLine);
            currentWayPoints.setMarks(marks);
            marks.Clear();
            currentWayPoints.setLines(lines);
            lines.Clear();
            isPathing = false;
            if (selectedUnit.TryGetComponent<Viking>(out Viking v))
            {
                v.btnDraw.interactable = false;
                v.btnRun.interactable = true;
            }
        }
    }
}

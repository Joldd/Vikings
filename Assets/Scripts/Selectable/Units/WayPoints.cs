using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    [SerializeField] Mark markPF;
    [SerializeField] LineRenderer linePF;
    public List<Mark> marks;
    public List<LineRenderer> lines;
    public Color lineColor;
    public Troop myTroop;
    public bool isNew;
    public float lineWidth = 0.2f;
    private GameManager gameManager;
    public bool isModifiable = true;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager.selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
        {
            if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
            {
                myTroop = messenger.troopSelected;
                myTroop.changingWayPoints = this;
            }
            else
            {
                myTroop = gameManager.selectedUnit.GetComponent<Troop>();
                myTroop.myWayPoints = this;
            }
        }
    }

    public void setMarks(List<Mark> m)
    {
        marks = new List<Mark>(m);
    }

    public void setLines(List<LineRenderer> l)
    {
        lines = new List<LineRenderer>(l);
    }

    public Mark nextPoint(Mark currentMark)
    {
        int index = marks.IndexOf(currentMark);
        if (index < marks.Count - 1)
        {
            return marks[index + 1];           
        }
        else
        {
            return currentMark;
        }
    }

    public LineRenderer nextLine(LineRenderer currentLine)
    {
        int index = lines.IndexOf(currentLine);
        if (index < lines.Count - 1)
        {
            return lines[index + 1];
        }
        else
        {
            return currentLine;
        }
    }

    public void ChangeColor(Color newColor)
    {
        lineColor = newColor;
        foreach (LineRenderer line in lines)
        {
            line.material.color = newColor;
        }
    }

    public void AddMark(Mark mark)
    {
        marks.Add(mark);
        mark.myWayPoints = this;
    }

    public void AddNewMark(Mark mark)
    {   
        Mark newMark = Instantiate(markPF, transform);
        marks.Insert(marks.IndexOf(mark), newMark);
        newMark.myWayPoints = this;
        newMark.transform.position = new Vector3(mark.transform.position.x - 0.5f, mark.transform.position.y, mark.transform.position.z - 0.5f);

        LineRenderer newLine = Instantiate(linePF, transform);
        lines.Insert(marks.IndexOf(newMark), newLine);
        newLine.startWidth = lineWidth;
        newLine.endWidth = lineWidth;
        newLine.material.color = lineColor;
        newLine.SetPosition(1, mark.transform.position);
        newLine.SetPosition(0, newMark.transform.position);

        lines[lines.IndexOf(newLine) - 1].SetPosition(1, newMark.transform.position);
    }

    private void Update()
    {
        for (int i = marks.Count - 1; i >= 0; i--)
        {
            Mark mark = marks[i].GetComponent<Mark>();
            if (mark.isMoving)
            {
                lines[i-1].SetPosition(1, mark.transform.position);
                if (i == marks.Count - 1) break;
                lines[i].SetPosition(0, mark.transform.position);
            }
            if (mark.isNavHit)
            {
                mark.isNavHit = false;
                lines[i - 1].SetPosition(1, mark.transform.position);
                if (i == marks.Count - 1) break;
                lines[i].SetPosition(0, mark.transform.position);
            }
            if (mark.deleted)
            {
                marks.RemoveAt(i);
                Destroy(mark.gameObject);
                if (i != marks.Count)
                {
                    LineRenderer l = lines[i];
                    lines.Remove(l);
                    Destroy(l.gameObject);
                    lines[i - 1].SetPosition(1, marks[i].transform.position);
                }
                else
                {
                    LineRenderer l = lines[i-1];
                    lines.Remove(l);
                    Destroy(l.gameObject);
                }      
            }
        }

        //////////////////  MESSAGER WAYPOINT  //////////////////////////
        if (isNew && marks.Count > 0)
        {

            marks[0].transform.position = myTroop.transform.position;
            lines[0].SetPosition(0, marks[0].transform.position);
        }
    }
}

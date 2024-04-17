using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public List<GameObject> marks;
    public List<LineRenderer> lines;
    public Color lineColor;
    Troop myTroop;
    public bool isNew;

    private void Start()
    {
        if (GameManager.Instance.selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
        {
            if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
            {
                myTroop = messenger.troopSelected;
                myTroop.changingWayPoints = this;
            }
            else
            {
                myTroop = GameManager.Instance.selectedUnit.GetComponent<Troop>();
                myTroop.myWayPoints = this;
            }
        }
    }

    public void setMarks(List<GameObject> m)
    {
        marks = new List<GameObject>(m);
    }

    public void setLines(List<LineRenderer> l)
    {
        lines = new List<LineRenderer>(l);
    }

    public GameObject nextPoint(GameObject currentMark)
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

    private void Update()
    {
        for (int i = marks.Count - 1; i >= 0; i--)
        {
            Mark mark = marks[i].GetComponent<Mark>();
            if (mark.isDragging)
            {
                lines[i-1].SetPosition(1, mark.transform.position);
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
                    Destroy(l);
                    lines[i - 1].SetPosition(1, marks[i].transform.position);
                }
                else
                {
                    LineRenderer l = lines[i-1];
                    lines.Remove(l);
                    Destroy(l);
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

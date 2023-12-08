using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public List<GameObject> marks;
    public List<LineRenderer> lines;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public List<GameObject> marks;

    public void setMarks(List<GameObject> m)
    {
        marks = new List<GameObject>(m);
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
}

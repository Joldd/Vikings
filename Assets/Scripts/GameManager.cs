using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] WayPoints wayPoints;
    [SerializeField] GameObject viking;
    [SerializeField] GameObject mark;
    List<GameObject> marks;
    GameObject currentMark;
    bool isPathing;
    Vector3 mouseScreenPostion;
    [SerializeField] GameObject floor;

    public void createPath()
    {
        isPathing = true;
        Instantiate(wayPoints);
        currentMark = Instantiate(mark);
        marks.Add(currentMark);
    }

    public void createViking()
    {

        if (GameObject.FindObjectOfType<WayPoints>() != null)
        {
            Instantiate(viking);
        }
    }

    private void Update()
    {
        
        if (isPathing)
        {
            mouseScreenPostion = Input.mousePosition;
            mouseScreenPostion.z = Camera.main.nearClipPlane + 1;
            currentMark.transform.position = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        }
        if(isPathing && Input.GetMouseButtonDown(0))
        {

        }
    }
}

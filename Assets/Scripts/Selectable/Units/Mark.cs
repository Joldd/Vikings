using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    int layer_mask;
    public bool isDragging;
    public bool deleted;
    Vector3 baseScale;
    float lastClickTime;
    public WayPoints myWayPoints;

    const float DOUBLE_CLICK_TIME = 0.2f;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        layer_mask = LayerMask.GetMask("Floor");
        baseScale = transform.localScale;
    }

    private void OnMouseDrag()
    {
        if (gameManager.isPathing) return;

        if (myWayPoints.myTroop.state != State.WAITING && myWayPoints == myWayPoints.myTroop.myWayPoints) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layer_mask))
        {
            transform.position = hit.point;
        }
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseOver()
    {
        if (gameManager.isPathing) return;

        if (myWayPoints.myTroop.state != State.WAITING && myWayPoints == myWayPoints.myTroop.myWayPoints) return;

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleted = true;
        }

        //DOUBLE CLICK => Create Mark
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if(timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                myWayPoints.AddNewMark(this);
            }

            lastClickTime = Time.time;
        }
    }

    private void OnMouseEnter()
    {
        if (myWayPoints.myTroop.state != State.WAITING && myWayPoints == myWayPoints.myTroop.myWayPoints) return;

        transform.localScale = 2*baseScale;
    }

    private void OnMouseExit()
    {
        transform.localScale = baseScale;
    }
}

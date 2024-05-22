using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    public bool isDragging;
    public bool deleted;
    Vector3 baseScale;
    float lastClickTime;
    public WayPoints myWayPoints;

    const float DOUBLE_CLICK_TIME = 0.2f;

    private GameManager gameManager;

    public bool canBuild = true;
    Vector3 startPosition;
    public bool goback;

    private bool isHover;

    private void Start()
    {
        gameManager = GameManager.Instance;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (gameManager.isPathing) return;

        if (myWayPoints.myTroop.state != State.WAITING && myWayPoints == myWayPoints.myTroop.myWayPoints) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, gameManager.layer_mark))
        {
            if(hit.transform.gameObject.GetComponent<Mark>() == this)
            {
                //ENTER
                if (!isHover)
                {
                    transform.localScale = 2 * baseScale;
                    isHover = true;
                }

                //CLICK && DOUBLECLICK
                if (Input.GetMouseButtonDown(0))
                {
                    float timeSinceLastClick = Time.time - lastClickTime;
                    startPosition = transform.position;
                    isDragging = true;

                    if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
                    {
                        myWayPoints.AddNewMark(this);
                    }

                    lastClickTime = Time.time;             
                }

                //DELETE
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    deleted = true;
                }

                //MOUSEUP
                if (Input.GetMouseButtonUp(0))
                {
                    if (!canBuild)
                    {
                        transform.position = startPosition;
                        goback = true;
                    }
                    isDragging = false;
                }             
            }
        }
        else
        {
            //EXIT
            if (isHover)
            {
                transform.localScale = baseScale;
                isHover = false;
            }
        }

        //DRAG
        if (isDragging)
        {
            RaycastHit hitFloor;
            if (Physics.Raycast(ray, out hitFloor, 100f, gameManager.layer_mask))
            {
                transform.position = hitFloor.point;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EntityHouse>(out EntityHouse house))
        {
            canBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EntityHouse>(out EntityHouse house))
        {
            canBuild = true;
        }
    }
}

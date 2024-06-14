using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Mark : MonoBehaviour
{
    public bool isMoving;
    public bool deleted;
    Vector3 baseScale;
    float lastClickTime;
    public WayPoints myWayPoints;

    private GameManager gameManager;

    public bool canBuild = true;
    Vector3 startPosition;
    public bool goback;

    private bool isHover;

    public bool isNavHit;

    private void Start()
    {
        gameManager = GameManager.Instance;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (gameManager.isPathing) return;
        if (myWayPoints.myTroop.state != State.WAITING && myWayPoints == myWayPoints.myTroop.myWayPoints) return;
        if (myWayPoints.marks[0] == this) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!myWayPoints.isModifiable) return;

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
                    gameManager.isOverMark = true;
                }

                //CLICK && DOUBLECLICK
                if (Input.GetMouseButtonDown(0))
                {
                    if (isMoving)
                    {
                        if (!canBuild)
                        {
                            transform.position = startPosition;
                            goback = true;
                        }
                        NavMeshHit navHit;
                        if (NavMesh.SamplePosition(transform.position, out navHit, 100f, NavMesh.AllAreas))
                        {
                            transform.position = navHit.position;
                            isNavHit = true;
                        }
                        isMoving = false;
                    }
                    else
                    {
                        if (myWayPoints.marks[myWayPoints.marks.Count - 1] == this)
                        {
                            gameManager.ContinuePathing(this);
                        }
                        else
                        {
                            startPosition = transform.position;
                            isMoving = true;
                        }
                    }

                    float timeSinceLastClick = Time.time - lastClickTime;
                    if (timeSinceLastClick <= gameManager.DOUBLE_CLICK_TIME)
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
                if (Input.GetMouseButtonDown(1))
                {
                    float timeSinceLastClick = Time.time - lastClickTime;

                    if (timeSinceLastClick <= gameManager.DOUBLE_CLICK_TIME)
                    {
                        gameManager.isOverMark = false;
                        deleted = true;
                    }

                    lastClickTime = Time.time;
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
                gameManager.isOverMark = false;
            }
        }

        //MOVE THE MARK
        if (isMoving)
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
            Debug.Log("hoho");
            canBuild = true;
        }
    }
}

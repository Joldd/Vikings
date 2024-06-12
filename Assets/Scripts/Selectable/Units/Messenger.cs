using UnityEngine;

public class Messenger : EntityUnit
{
    public Troop troopSelected;
    private bool bringMessage;
    private bool backHome;
    private Vector3 homePos;
    public bool canMsg = true;
    public bool canGo;
    public bool troopChoosen;
    private GameManager gameManager;

    public override void Start()
    {
        base.Start();

        gameManager = GameManager.Instance;

        homePos = transform.position;
    }

    public void Select()
    {
        if (troopSelected)
        {
            if (troopSelected.myWayPoints)
            {
                troopSelected.myWayPoints.gameObject.SetActive(true);
            }
            if (troopSelected.changingWayPoints)
            {
                troopSelected.changingWayPoints.gameObject.SetActive(true);
            }
        }

    }

    public void UnSelect()
    {
        if (troopSelected)
        {
            if (troopSelected.myWayPoints)
            {
                troopSelected.myWayPoints.gameObject.SetActive(false);
            }
            if (troopSelected.changingWayPoints)
            {
                troopSelected.changingWayPoints.gameObject.SetActive(false);
            }
        }
        else
        {
            canGo = false;
            canMsg = true;
            troopChoosen = false;
        }

    }

    private void QuitTroop()
    {
        if (troopSelected)
        {
            if (troopSelected.myWayPoints)
            {
                troopSelected.myWayPoints.gameObject.SetActive(false);
            }
            if (troopSelected.changingWayPoints)
            {
                troopSelected.changingWayPoints.gameObject.SetActive(false);
            }
            troopSelected = null;
        }
    }

    private void Update()
    {
        if (bringMessage)
        {
            myTroop.NavMeshAgent.SetDestination(troopSelected.transform.position);
            animator.Play("Run");
            if (Vector3.Distance(transform.position, troopSelected.transform.position) <= 1.5f )
            {
                bringMessage = false;
                if (troopSelected.myWayPoints)
                {
                    gameManager.L_WayPoints.Remove(troopSelected.myWayPoints);
                    Destroy(troopSelected.myWayPoints.gameObject);
                }
                troopSelected.myWayPoints = troopSelected.changingWayPoints;
                troopSelected.Run();
                backHome = true;
            }
        }
        else if (backHome)
        {
            QuitTroop();
            animator.Play("Run");
            myTroop.NavMeshAgent.SetDestination(homePos);
            if (Vector3.Distance(transform.position, homePos) < 1.5f)
            {
                backHome = false;
                canMsg = true;
            }
        }
        else
        {
            animator.Play("Idle");
        }
    }

    public void ChooseTroop()
    {
        if (!troopChoosen)
        {
            troopChoosen = true;
            canMsg = false;
        }
    }

    public void StopChooseTroop()
    {
        troopChoosen = !troopChoosen;
        gameManager.CreateNewPath();
    }

    public void Go()
    {
        bringMessage = true;
        canGo = false;
    }

    public void Reset()
    {
        canMsg = true;
        canGo = false;
        troopChoosen = false;
        ChooseTroop();
        gameManager.ChangeCursor(gameManager.cursorMsg);
    }
}

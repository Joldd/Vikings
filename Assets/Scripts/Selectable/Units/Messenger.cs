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

        healthBar.gameObject.SetActive(false);
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
            transform.position = Vector3.MoveTowards(transform.position, troopSelected.transform.position, Speed * Time.deltaTime);

            animator.Play("Run");
            transform.LookAt(troopSelected.transform);
            if (Vector3.Distance(transform.position, troopSelected.transform.position) < 0.1f )
            {
                bringMessage = false;
                if (troopSelected.myWayPoints)
                {
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
            transform.LookAt(homePos);
            transform.position = Vector3.MoveTowards(transform.position, homePos, Speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, homePos) < 0.1f)
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
}

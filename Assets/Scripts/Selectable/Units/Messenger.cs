using UnityEngine;
using UnityEngine.UI;

public class Messenger : EntityUnit
{
    public Troop troopSelected;
    private Color startColor;
    [SerializeField] private Color chooseColor;
    private bool bringMessage;
    private bool backHome;
    private Vector3 homePos;
    [SerializeField] private Button messageBtn;

    public override void Start()
    {
        base.Start();

        startColor = myTroop.panelMsg.color;

        homePos = transform.position;

        healthBar.gameObject.SetActive(false);

        myTroop.btnDrawMsg.onClick.AddListener(() => ChooseTroop());
        myTroop.btnGoMsg.onClick.AddListener(() => Go());
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
        if(bringMessage)
        {
            transform.position = Vector3.MoveTowards(transform.position, troopSelected.transform.position, speed * Time.deltaTime);
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
            transform.position = Vector3.MoveTowards(transform.position, homePos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, homePos) < 0.1f)
            {
                backHome = false;
                myTroop.btnDrawMsg.interactable = true;
            }
        }
        else
        {
            animator.Play("Idle");
        }
    }

    public void ChooseTroop()
    {
        if (!GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = true;
            myTroop.panelMsg.color = chooseColor;
        }
        else if (GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = false;
            myTroop.panelMsg.color = startColor;
        }
    }

    public void StopChooseTroop()
    {
        GameManager.Instance.isChoosingMessager = !GameManager.Instance.isChoosingMessager;
        myTroop.panelMsg.color = startColor;
        GameManager.Instance.CreateNewPath();
        myTroop.btnDrawMsg.interactable = false;
    }

    public void Go()
    {
        bringMessage = true;
    }
}

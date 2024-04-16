using UnityEngine;
using UnityEngine.UI;

public class Messenger : Selectable
{
    public Troop troopSelected;
    [SerializeField] private Image panel;
    private Color startColor;
    [SerializeField] private Color chooseColor;
    private bool bringMessage;
    private bool backHome;
    [SerializeField] private float speed;
    private Vector3 homePos;
    private GameObject body;
    private Animator _anim;
    [SerializeField] private Button messageBtn;

    public override void Start()
    {
        base.Start();
        startColor = panel.color;
        homePos = transform.position;

        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>();
    }

    public override void Select()
    {
        base.Select();

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

    public override void UnSelect()
    {
        base.UnSelect();

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
            _anim.Play("Run");
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
            _anim.Play("Run");
            transform.LookAt(homePos);
            transform.position = Vector3.MoveTowards(transform.position, homePos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, homePos) < 0.1f)
            {
                backHome = false;
                messageBtn.interactable = true;
            }
        }
        else
        {
            _anim.Play("Idle");
        }
    }

    public void ChooseTroop()
    {
        if (!GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = true;
            panel.color = chooseColor;
        }
        else if (GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = false;
            panel.color = startColor;
        }
    }

    public void StopChooseTroop()
    {
        GameManager.Instance.isChoosingMessager = !GameManager.Instance.isChoosingMessager;
        panel.color = startColor;
        GameManager.Instance.createNewPath();
        messageBtn.interactable = false;
    }

    public void Go()
    {
        bringMessage = true;
    }
}

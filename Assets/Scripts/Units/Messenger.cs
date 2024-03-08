using UnityEngine;
using UnityEngine.UI;

public class Messenger : Selectable
{
    public Viking vikingSelected;
    [SerializeField] private Image panel;
    private Color startColor;
    [SerializeField] private Color chooseColor;
    private bool bringMessage;
    private bool backHome;
    [SerializeField] private float speed;
    private Vector3 homePos;
    private GameObject body;
    private Animator _anim;

    public override void Start()
    {
        base.Start();
        startColor = panel.color;
        homePos = transform.position;

        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        if(bringMessage)
        {
            transform.position = Vector3.MoveTowards(transform.position, vikingSelected.transform.position, speed * Time.deltaTime);
            _anim.Play("Run");
            transform.LookAt(vikingSelected.transform);
            if (Vector3.Distance(transform.position, vikingSelected.transform.position) < 0.1f )
            {
                bringMessage = false;
                if (vikingSelected.myWayPoints)
                {
                    Destroy(vikingSelected.myWayPoints.gameObject);
                }
                vikingSelected.myWayPoints = vikingSelected.changingWayPoints;
                vikingSelected.Run();
                backHome = true;
            }
        }
        else if (backHome)
        {
            _anim.Play("Run");
            transform.LookAt(homePos);
            transform.position = Vector3.MoveTowards(transform.position, homePos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, homePos) < 0.1f)
            {
                backHome = false;
            }
        }
        else
        {
            _anim.Play("Idle");
        }
    }

    public void ChooseViking()
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

    public void StopChooseViking()
    {
        GameManager.Instance.isChoosingMessager = !GameManager.Instance.isChoosingMessager;
        panel.color = startColor;
        GameManager.Instance.createNewPath();
    }

    public void Go()
    {
        bringMessage = true;
    }
}

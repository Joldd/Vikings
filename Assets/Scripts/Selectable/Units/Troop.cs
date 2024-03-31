using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    SLEEPING = 00,
    RUNNING = 01,
    RUNATTACK = 02,
    ATTACK = 03,
    WAITING = 04,
    ENEMY = 05
}

public class Troop : Selectable
{
    public List<Viking> L_Vikings = new List<Viking>();
    private float radius = 0.8f;
    public Type type;
    public State state;
    public bool checkEnemy;
    public bool isOver;

    public Button btnDraw;
    public Button btnRun;
    private GameObject canvasUnit;

    //WAYpoints
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
    GameObject currentMark;
    LineRenderer currentLine;

    //STATS
    private int speed;

    public override void Start()
    {
        base.Start();

        speed = L_Vikings[0].speed;

        canvasUnit = transform.Find("CanvasUnit").gameObject;
        btnDraw = canvasUnit.transform.Find("Buttons").Find("Draw").GetComponent<Button>();
        btnRun = canvasUnit.transform.Find("Buttons").Find("Run").GetComponent<Button>();

        btnDraw.onClick.AddListener(() =>
        {
            GameManager.Instance.createPath();
        });
        btnRun.interactable = false;

        canvasUnit.SetActive(false);
    }

    public void AddUnit(Viking viking)
    {
        viking.transform.SetParent(transform);
        L_Vikings.Add(viking);
        viking.myTroop = this;
        for (int i = 0; i < L_Vikings.Count; i++)
        {
            Viking v = L_Vikings[i];
            v.transform.position = new Vector3(transform.position.x + radius * Mathf.Cos(i * 2 * Mathf.PI / L_Vikings.Count), transform.position.y, transform.position.z + radius * Mathf.Sin(i * 2 * Mathf.PI / L_Vikings.Count));
        }
    }

    private void PlayAnimation(string name)
    {
        foreach (Viking viking in L_Vikings)
        {
            viking.animator.Play(name);
        }
    }

    public void Run()
    {
        state = State.RUNNING;
        currentMark = myWayPoints.marks[0];
        transform.position = currentMark.transform.position;
        currentMark.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
        transform.LookAt(currentMark.transform);
        PlayAnimation("Run");
        btnRun.interactable = false;
    }

    private void Update()
    {
        //OUTLINE
        

        //////////////////   STATE   ////////////////////////////////////
        if (state != State.SLEEPING)
        {
            if (tag != "Enemy" && currentLine)
            {
                currentLine.SetPosition(0, transform.position);
            }

            if (state == State.RUNNING && myWayPoints)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentMark.transform.position, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
                {
                    currentMark.SetActive(false);
                    currentMark = myWayPoints.nextPoint(currentMark);
                    currentLine.gameObject.SetActive(false);
                    currentLine = myWayPoints.nextLine(currentLine);
                    transform.LookAt(currentMark.transform);
                }
                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) == myWayPoints.marks.Count - 1)
                {
                    //Destroy(myWayPoints.gameObject);
                    state = State.WAITING;
                }
            }

            if (state == State.WAITING)
            {
                PlayAnimation("Idle");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = State.RUNATTACK;
            ChangeState(State.RUNATTACK);
            PlayAnimation("Run");
            GiveTarget(other.gameObject.GetComponent<Unit>());
            checkEnemy = true;
        }
    }

    public void ChangeState(State state)
    {
        foreach (Viking viking in L_Vikings)
        {
            viking.state = state;
        }
    }

    private void GiveTarget(Unit target)
    {
        foreach (Viking viking in L_Vikings)
        {
            viking.target = target;
        }
    }

    public void KillTarget(Unit target)
    {
        Destroy(target.gameObject);
        target = null;
        foreach (Viking viking in L_Vikings)
        {
            viking.target = null;
        }
    }
}

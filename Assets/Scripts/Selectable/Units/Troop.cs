using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    private Unit target;
    Vector3 directionEnemy;
    public House myHouse;

    public Button btnDraw;
    public Button btnRun;
    private GameObject canvasUnit;

    //Waypoints
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
    GameObject currentMark;
    LineRenderer currentLine;

    //STATS
    private int speed;
    private int range;
    private float timerAttackMax;
    private float timerAttack = 0f;
    public int maxTroop;

    public override void Start()
    {
        base.Start();

        speed = L_Vikings[0].speed;
        range = L_Vikings[0].range;
        timerAttackMax = L_Vikings[0].timerAttackMax;
        maxTroop = L_Vikings[0].maxTroop;

        canvasUnit = transform.Find("CanvasUnit").gameObject;
        btnDraw = canvasUnit.transform.Find("Buttons").Find("Draw").GetComponent<Button>();
        btnRun = canvasUnit.transform.Find("Buttons").Find("Run").GetComponent<Button>();

        btnDraw.onClick.AddListener(() =>
        {
            GameManager.Instance.createPath();
        });
        btnRun.interactable = false;

        canvasUnit.SetActive(false);

        if (tag == "Enemy")
        {
            state = State.ENEMY;
            directionEnemy = transform.forward;
        }
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

    private void Attack()
    {
        foreach (Viking viking in L_Vikings)
        {
            viking.Attack();
        }
    }

    private void GiveTarget()
    {
        foreach (Viking viking in L_Vikings)
        {
            viking.target = target;
        }
    }

    private void KillTarget()
    {
        Destroy(target.gameObject);
        target = null;
        foreach (Viking viking in L_Vikings)
        {
            viking.target = null;
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
        myHouse.DetachTroop(this);
    }

    private void Update()
    {
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

            //////////////// FIGHTING  ////////////////
            if (state == State.RUNATTACK)
            {
                if (target == null)
                {
                    state = State.RUNNING;
                    checkEnemy = false;

                    if (tag == "Enemy")
                    {
                        state = State.ENEMY;
                        transform.LookAt(directionEnemy);
                    }
                }
                else
                {
                    Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    if (Vector3.Distance(transform.position, target.transform.position) <= range)
                    {
                        state = State.ATTACK;
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 15f)
                    {
                        state = State.RUNNING;
                        checkEnemy = false;
                        if (tag == "Enemy")
                        {
                            state = State.ENEMY;
                            transform.LookAt(directionEnemy);
                        }
                    }
                }
            }
            if (state == State.ATTACK && target)
            {
                if (target.PV <= 0)
                {
                    //DEATH
                    if (target.TryGetComponent<Viking>(out Viking vikingTarget))
                    {
                        vikingTarget.Die();
                    }
                    if (target.TryGetComponent<UnitHouse>(out UnitHouse houseTarget))
                    {
                        houseTarget.Die();
                    }

                    KillTarget();

                    state = State.RUNATTACK;

                    if (tag == "Enemy")
                    {
                        state = State.ENEMY;
                        transform.LookAt(directionEnemy);
                    }
                }
                else
                {
                    timerAttack -= Time.deltaTime;
                    if (timerAttack <= 0)
                    {
                        Attack();
                        timerAttack = timerAttackMax;
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 5f)
                    {
                        timerAttack = timerAttackMax;
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                    }
                }
            }
            
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (state == State.ENEMY)
        {
            transform.position += speed * directionEnemy * Time.deltaTime;
            PlayAnimation("Run");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = State.RUNATTACK;
            PlayAnimation("Run");
            target = other.gameObject.GetComponent<Unit>();
            GiveTarget();
            checkEnemy = true;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Troop : Selectable
{
    public List<Viking> L_Vikings = new List<Viking>();
    private float radius = 0.8f;
    public Type type;
    public string state;
    private bool checkEnemy;
    private Selectable target;
    public bool isOver;

    private Button btnDraw;
    private Button btnRun;
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
        state = "Running";
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
        if (state != "Sleeping")
        {
            if (tag != "Enemy" && currentLine)
            {
                currentLine.SetPosition(0, transform.position);
            }

            if (state == "Running" && myWayPoints)
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
                    state = "Waiting";
                }
            }

            if (state == "Waiting")
            {
                PlayAnimation("Idle");
            }

            if (state == "RunAttack")
            {
                //if (target == null)
                //{
                //    state = "Running";
                //    checkEnemy = false;
                //    if (tag == "Enemy")
                //    {
                //        state = "Enemy";
                //        transform.LookAt(directionEnemy);
                //    }
                //}
                //else
                //{
                //    Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                //    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                //    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                //    if (Vector3.Distance(transform.position, target.transform.position) <= range)
                //    {
                //        state = "Attack";
                //    }
                //    if (Vector3.Distance(transform.position, target.transform.position) > 15f)
                //    {
                //        state = "Running";
                //        checkEnemy = false;
                //        if (tag == "Enemy")
                //        {
                //            state = "Enemy";
                //            transform.LookAt(directionEnemy);
                //        }
                //    }
                //}
            }
            if (state == "Attack")
            {
                //    if (target.PV <= 0)
                //    {
                //        //DEATH
                //        if (target.TryGetComponent<Viking>(out Viking vikingTarget))
                //        {
                //            vikingTarget.Die();
                //        }
                //        if (target.TryGetComponent<House>(out House houseTarget))
                //        {
                //            houseTarget.Die();
                //        }

                //        Destroy(target.gameObject);

                //        checkEnemy = false;

                //        if (myWayPoints)
                //        {
                //            state = "Running";
                //            _anim.Play("Run");
                //        }
                //        else
                //        {
                //            state = "Waiting";
                //        }

                //        if (tag == "Enemy")
                //        {
                //            state = "Enemy";
                //            transform.LookAt(directionEnemy);
                //        }
                //    }
                //    else
                //    {
                //        timerAttack -= Time.deltaTime;
                //        if (timerAttack <= 0)
                //        {
                //            Attack();
                //            timerAttack = timerAttackMax;
                //        }
                //        if (Vector3.Distance(transform.position, target.transform.position) > 5f)
                //        {
                //            timerAttack = timerAttackMax;
                //            state = "RunAttack";
                //            PlayAnimation("Run");
                //        }
                //    }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = "RunAttack";
            PlayAnimation("Run");
            target = other.gameObject.GetComponent<Selectable>();
            checkEnemy = true;
        }
    }

}

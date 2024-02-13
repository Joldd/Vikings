using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viking : Selectable
{
    public WayPoints myWayPoints;
    GameObject currentMark;
    LineRenderer currentLine;
    
    Animator _anim;
    string state = "Sleeping";

    public Selectable target;
    [SerializeField] float timerAttackMax;
    float timerAttack = 0f;

    [SerializeField] int speed;
    [SerializeField] int range;
    public int damage = 1;

    public Button btnDraw;
    public Button btnRun;

    GameObject body;

    GameObject HUDCharacter;

    [SerializeField] bool enemyStop;
    bool checkEnemy;

    public override void Start()
    {
        base.Start();

        body = transform.Find("Body").gameObject;
        body.SetActive(false);

        _anim = body.GetComponent<Animator>(); 

        canvas = transform.Find("CanvasUnit").gameObject;
        btnDraw = canvas.transform.Find("Buttons").Find("Draw").GetComponent<Button>();
        btnRun = canvas.transform.Find("Buttons").Find("Run").GetComponent<Button>();

        btnDraw.onClick.AddListener(() =>
        {
            GameManager.Instance.createPath();
        });
        btnRun.interactable = false;

        HUDCharacter = transform.Find("HUDCharacter").gameObject;
        HUDCharacter.gameObject.SetActive(false);
    }

    public override void Select()
    {
        base.Select();
        if (myWayPoints != null)
        {
            myWayPoints.gameObject.SetActive(true);
        }
    }

    public override void UnSelect()
    {
        base.UnSelect();
        if (myWayPoints != null)
        {
            myWayPoints.gameObject.SetActive(false);
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
        _anim.Play("Run");
        btnRun.interactable = false;
    }

    private void Attack()
    {
        _anim.speed = 1f / timerAttackMax;
        _anim.Play("Attack");
    }

    public override void Update()
    {
        base.Update();

        //////////////////  CONSTRUCTION   //////////////////////////////
        if (!isBuilt)
        {
            timeBuild -= Time.deltaTime;
        }
        if (timeBuild <= 0 && !isBuilt)
        {
            if (tag != "Enemy")
            {
                canBeSelected = true;
            }
            body.SetActive(true);
            HUDCharacter.gameObject.SetActive(true);
        }

        //////////////////   STATE   ////////////////////////////////////
        if (state != "Sleeping") 
        {
            if (tag != "Enemy")
            {
                currentLine.SetPosition(0, transform.position);
            }

            if (state == "Running")
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
                    _anim.Play("Idle");
                }
            } 
            
            if (state == "RunAttack")
            {
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                if (Vector3.Distance(transform.position, target.transform.position) <= range)
                {
                    state = "Attack";
                }
                if (Vector3.Distance(transform.position, target.transform.position) > 15f)
                {
                    state = "Running";
                }
            }
            if (state == "Attack")
            {
                if (target.PV <= 0)
                {
                    Destroy(target.gameObject);
                    checkEnemy = true;
                    state = "Running";
                    _anim.Play("Run");
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
                        state = "RunAttack";
                        _anim.Play("Run");
                    }
                }
            }
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (tag == "Enemy" && state != "Attack")
        {
            transform.position += speed * transform.forward * Time.deltaTime;
            _anim.Play("Run");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = "RunAttack";
            _anim.Play("Run");
            target = other.gameObject.GetComponent<Selectable>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = "RunAttack";
            _anim.Play("Run");
            target = other.gameObject.GetComponent<Selectable>();
            checkEnemy = false;
        }
    }
}

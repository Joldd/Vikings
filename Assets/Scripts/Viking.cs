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

    public Enemy target;
    [SerializeField] float timerAttackMax;
    float timerAttack = 0f;

    [SerializeField] int speed;
    public float damage = 1f;

    public Button btnDraw;
    public Button btnRun;

    GameObject body;

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
        _anim.speed = 1f/timerAttackMax;
        _anim.Play("Attack");
        
    }

    private void Update()
    {
        //////////////////  CONSTRUCTION   //////////////////////////////
        if (timeBuild <= 0 && !isBuilt)
        {
            isBuilt = true;
            canBeSelected = true;
            body.SetActive(true);
        }
        else
        {
            timeBuild -= Time.deltaTime;
        }



        //////////////////   STATE   ////////////////////////////////////
        if (state != "Sleeping") 
        {
            currentLine.SetPosition(0, transform.position);

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
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                if (Vector3.Distance(transform.position, target.transform.position) < 1f)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            state = "RunAttack";
            target = other.gameObject.GetComponent<Enemy>();
        }
    }
}

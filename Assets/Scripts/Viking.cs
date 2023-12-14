using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : MonoBehaviour
{
    WayPoints myWayPoints;
    [SerializeField] int speed;
    GameObject currentMark;
    LineRenderer currentLine;
    [SerializeField] Animator _anim;
    string state = "Running";
    Enemy target;
    [SerializeField] float timerAttackMax;
    float timerAttack = 0f;
    [SerializeField] float damage = 1f;
    bool firstAttack = true;

    private void Start()
    {
        myWayPoints = GameManager.Instance.currentWayPoints;
        currentMark = myWayPoints.marks[0];
        transform.position = currentMark.transform.position;
        currentMark.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
        transform.LookAt(currentMark.transform);
    }

    private void Attack()
    {
        _anim.speed = 1f/timerAttackMax;
        _anim.Play("Attack",0);
        
    }

    public void Deal()
    {
        Debug.Log("azd");
    }

    private void Update()
    {
        if (myWayPoints != null) 
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
                    Destroy(myWayPoints.gameObject);
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
                    if (!firstAttack)
                    {
                        target.PV--;
                        target.updateHealthBar();
                    }
                    firstAttack = false;
                    Attack();
                    timerAttack = timerAttackMax;
                }
                if (Vector3.Distance(transform.position, target.transform.position) > 5f)
                {
                    timerAttack = timerAttackMax;
                    state = "RunAttack";
                    firstAttack = true;
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

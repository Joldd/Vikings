using UnityEngine;
using UnityEngine.UI;

public enum Type
{
    Paladin,
    Guard,
    Mutant
}

public class Viking : Selectable
{
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
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

    [SerializeField] bool enemyStop;
    bool checkEnemy;
    Vector3 directionEnemy;

    public Constructible areaToCapture;

    [SerializeField] int goldToWin;

    public Type type;

    public Troop myTroop;

    public override void Start()
    {
        base.Start();

        //Health Bar
        healthBar = Instantiate(healthBar, transform.position, Quaternion.identity);
        healthBar.StartBar(gameObject);
        healthBar.UpdateValue();

        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>(); 

        canvas = transform.Find("CanvasUnit").gameObject;
        btnDraw = canvas.transform.Find("Buttons").Find("Draw").GetComponent<Button>();
        btnRun = canvas.transform.Find("Buttons").Find("Run").GetComponent<Button>();

        btnDraw.onClick.AddListener(() =>
        {
            GameManager.Instance.createPath();
        });
        btnRun.interactable = false;

        if (tag == "Enemy")
        {
            state = "Enemy";
            directionEnemy = transform.forward;
        }
    }

    public override void Select()
    {
        base.Select();
        if (myWayPoints)
        {
            myWayPoints.gameObject.SetActive(true);
        }
        if (changingWayPoints)
        {
            changingWayPoints.gameObject.SetActive(true);
        }
        if (myTroop)
        {
            myTroop.Select();
        }
    }

    public override void UnSelect()
    {
        base.UnSelect();
        if (myWayPoints)
        {
            myWayPoints.gameObject.SetActive(false);
        }
        if (changingWayPoints)
        {
            changingWayPoints.gameObject.SetActive(false);
        }
        if (myTroop)
        {
            myTroop.UnSelect();
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

    public override void Die()
    {
        base.Die();

        if (areaToCapture)
        {
            if (tag == "Enemy") areaToCapture.enemyCapturing = false;
            if (tag == "Player") areaToCapture.playerCapturing = false;
        }
        if (myWayPoints)
        {
            Destroy(myWayPoints.gameObject);
        }

        if (tag == "Enemy")
        {
            GameManager.Instance.gold += goldToWin;
            GameManager.Instance.updateRessources();
        }
    }

    private void Update()
    {
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
                _anim.Play("Idle");
            }
            
            if (state == "RunAttack")
            {
                if (target == null)
                {
                    state = "Running";
                    checkEnemy = false;
                    if (tag == "Enemy")
                    {
                        state = "Enemy";
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
                        state = "Attack";
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 15f)
                    {
                        state = "Running";
                        checkEnemy = false;
                        if (tag == "Enemy")
                        {
                            state = "Enemy";
                            transform.LookAt(directionEnemy);
                        }
                    }
                }
            }
            if (state == "Attack")
            {
                if (target.PV <= 0)
                {
                    //DEATH
                    if (target.TryGetComponent<Viking>(out Viking vikingTarget))
                    {
                        vikingTarget.Die();
                    }
                    if (target.TryGetComponent<House>(out House houseTarget))
                    {
                        houseTarget.Die();
                    }

                    Destroy(target.gameObject);
                    
                    checkEnemy = false;

                    if (myWayPoints)
                    {
                        state = "Running";
                        _anim.Play("Run");
                    }
                    else
                    {
                        state = "Waiting";
                    }                 

                    if (tag == "Enemy")
                    {
                        state = "Enemy";
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
                        state = "RunAttack";
                        _anim.Play("Run");
                    }
                }
            }
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (state == "Enemy")
        {
            transform.position += speed * directionEnemy * Time.deltaTime;
            _anim.Play("Run");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = "RunAttack";
            _anim.Play("Run");
            target = other.gameObject.GetComponent<Selectable>();
            checkEnemy = true;
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Type
{
    Paladin,
    Guard,
    Mutant
}

public class Viking : Unit
{
    public State state = State.SLEEPING;

    public Unit target;
    [SerializeField] float timerAttackMax;
    float timerAttack = 0f;

    public int speed;
    [SerializeField] int range;
    public int damage = 1;

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

        healthBar.UpdateValue();

        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();
        
        if (tag == "Enemy")
        {
            state = State.ENEMY;
            directionEnemy = transform.forward;
        }
    }

    private void Attack()
    {
        animator.speed = 1f / timerAttackMax;
        animator.Play("Attack");
    }

    public override void Die()
    {
        base.Die();

        if (areaToCapture)
        {
            if (tag == "Enemy") areaToCapture.enemyCapturing = false;
            if (tag == "Player") areaToCapture.playerCapturing = false;
        }

        if (tag == "Enemy")
        {
            GameManager.Instance.gold += goldToWin;
            GameManager.Instance.updateRessources();
        }

        if (myTroop) myTroop.L_Vikings.Remove(this);
    }

    private void Update()
    {
        //////////////////   STATE   ////////////////////////////////////
        if (state != State.SLEEPING) 
        {         
            if (state == State.RUNATTACK)
            {
                if (target == null)
                {
                    state = State.RUNNING;
                    checkEnemy = false;

                    if (myTroop)
                    {
                        myTroop.state = State.RUNNING;
                        myTroop.checkEnemy = false;
                    }

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
                    
                    if (myTroop)
                    {
                        myTroop.KillTarget(target);
                        myTroop.state = State.RUNNING;
                    }
                    else
                    {
                        Destroy(target.gameObject);
                    }

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
                        animator.Play("Run");
                    }
                }
            }
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (state == State.ENEMY)
        {
            transform.position += speed * directionEnemy * Time.deltaTime;
            animator.Play("Run");
        }
    }


    //JUST FOR ENEMY ACTUALLY 
    private void OnTriggerStay(Collider other)
    {
        if (tag == "Player") return;
        if (checkEnemy) return;

        if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        {
            state = State.RUNATTACK;
            animator.Play("Run");
            target = other.gameObject.GetComponent<Unit>();
            checkEnemy = true;
        }
    }
}

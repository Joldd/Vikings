using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static FischlWorks_FogWar.csFogWar;

public enum FlankValues
{
    FRONT = 1,
    SIDES = 2,
    BACK = 3
}
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
    public List<EntityUnit> L_Units = new List<EntityUnit>();
    private EntityUnit unitRef;
    private float radius = 0.8f;
    public Type type;
    public State state;
    public bool checkEnemy;
    public bool isOver;
    private Entity target;
    Vector3 directionEnemy;
    public House myHouse;

    //UI
    [SerializeField] private GameObject canvasUnit;
    //UI Units
    [SerializeField] private GameObject btnsUnit;
    public Button btnDraw;
    public Button btnRun;
    //UI Messenger
    public Image panelMsg;
    [SerializeField] private GameObject btnsMsg;
    public Button btnDrawMsg;
    public Button btnGoMsg;

    //Waypoints
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
    GameObject currentMark;
    LineRenderer currentLine;

    //STATS
    private int speed;
    private float range;
    private int aoeRange;
    private int flankRange;
    private float timerAttackMax;
    private float timerAttack = 0f;
    public int maxTroop;

    [SerializeField] private LayerMask layerMaskTroop; 
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    //FOGWAR
    [SerializeField] GameObject ward;
    private float timerWardMax = 2f;
    private float timerWard;

    public override void Start()
    {
        base.Start();

        outline.enabled = false;

        speed = unitRef.speed;
        navMeshAgent.speed = speed;
        range = unitRef.range;
        aoeRange = unitRef.aoeRange;
        flankRange = unitRef.flankRange;
        timerAttackMax = unitRef.timerAttackMax;
        maxTroop = unitRef.maxTroop;

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

        if (type == Type.Messenger)
        {
            btnsMsg.SetActive(true);
            btnsUnit.SetActive(false);
        }
        else
        {
            btnsMsg.SetActive(false);
            btnsUnit.SetActive(true);
        }
    }

    public void AddUnit(EntityUnit unit)
    {
        unitRef = unit;
        unit.transform.SetParent(transform);
        L_Units.Add(unit);
        unit.myTroop = this;
        for (int i = 0; i < L_Units.Count; i++)
        {
            EntityUnit v = L_Units[i];
            v.transform.position = L_Units.Count > 1 ? new Vector3(transform.position.x + radius * Mathf.Cos(i * 2 * Mathf.PI / L_Units.Count), transform.position.y, transform.position.z + radius * Mathf.Sin(i * 2 * Mathf.PI / L_Units.Count)) : transform.position;
        }
        noOutLine();
    }

    public override void noOutLine()
    {
        foreach (EntityUnit unit in L_Units)
        {
            if (unit.outline)
            {
                unit.outline.OutlineMode = Outline.Mode.Nothing;
            }
        }
    }

    public override void hoverOutline()
    {
        if (tag == "Enemy") return;

        foreach (EntityUnit unit in L_Units)
        {
            if (unit.outline)
            {
                unit.outline.OutlineMode = Outline.Mode.OutlineAll;
                unit.outline.OutlineWidth = 2;
            }
        }
    }

    public override void selectOutline()
    {
        if (tag == "Enemy") return;

        foreach (EntityUnit unit in L_Units)
        {
            if (unit.outline)
            {
                unit.outline.OutlineMode = Outline.Mode.OutlineAll;
                unit.outline.OutlineWidth = 4;
            }
        }
    }

    public override void Select()
    {
        base.Select();

        if (myWayPoints) myWayPoints.gameObject.SetActive(true);

        if (type == Type.Messenger)
        {
            if (L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
            {
                messenger.Select();
            }
        }
    }

    public override void UnSelect()
    {
        base.UnSelect();

        if (myWayPoints) myWayPoints.gameObject.SetActive(false);

        if (type == Type.Messenger)
        {
            if (L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
            {
                messenger.UnSelect();
            }
        }
    }

    private void PlayAnimation(string name)
    {
        foreach (EntityUnit unit in L_Units)
        {
            unit.animator.Play(name);
        }
    }

    private void Attack()
    {
        foreach (EntityUnit unit in L_Units)
        {
            unit.Attack();
        }
    }

    private void GiveTarget(FlankValues flankValue = FlankValues.FRONT)
    {
        foreach (EntityUnit unit in L_Units)
        {
            unit.target = target;
            //TODO Give bonus to units
        }
    }

    private EntityUnit GetNearestUnitFromTroop(Vector3 pos)
    {
        EntityUnit nearestUnit = null;
        foreach (EntityUnit unit in L_Units)
        {
            Vector3 unitPos = unit.transform.position;
            if (nearestUnit != null)
            {
                nearestUnit = Vector3.Distance(pos, unitPos) < Vector3.Distance(nearestUnit.transform.position, unitPos) ? unit : nearestUnit;
            }
            else
            {
                nearestUnit = unit;
            }
        }

        return nearestUnit;
    }

    private void GetTargeted(Troop enemyTroop)
    {
        if (target == null)
        {
            target = enemyTroop.GetNearestUnitFromTroop(transform.position);
            checkEnemy = true;
            GiveTarget();
            state = State.RUNATTACK;
        }
    }

    private void KillTarget()
    {
        Destroy(target.gameObject);
        target = null;
        foreach (EntityUnit unit in L_Units)
        {
            unit.target = null;
        }
    }

    public void Run()
    {
        state = State.RUNNING;
        currentMark = myWayPoints.marks[0];
        navMeshAgent.SetDestination(currentMark.transform.position);
        // transform.position = currentMark.transform.position;
        currentMark.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
        //transform.LookAt(currentMark.transform);
        btnRun.interactable = false;
        if (myHouse) myHouse.DetachTroop(this);

    }

    public void RemoveUnit(EntityUnit unit)
    {
        L_Units.Remove(unit);
        if (L_Units.Count <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 3;
        Debug.DrawRay(transform.position, forward, Color.red);
        //////////////////   STATE   ////////////////////////////////////
        if (state != State.SLEEPING)
        {
            if (tag != "Enemy" && currentLine)
            {
                currentLine.SetPosition(0, transform.position);
            }

            if (state == State.RUNNING && myWayPoints)
            {
                PlayAnimation("Run");
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(currentMark.transform.position);
                // transform.position = Vector3.MoveTowards(transform.position, currentMark.transform.position, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.1f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
                {
                    currentMark.SetActive(false);
                    currentMark = myWayPoints.nextPoint(currentMark);
                    currentLine.gameObject.SetActive(false);
                    currentLine = myWayPoints.nextLine(currentLine);
                    // transform.LookAt(currentMark.transform);
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
                    PlayAnimation("Run");
                    navMeshAgent.enabled = false;
                    Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    // navMeshAgent.SetDestination(targetPos);
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
                    target.Die();

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

        //VISION FOGWAR
        if (state != State.ENEMY && state != State.SLEEPING && state != State.WAITING)
        {
            timerWard -= Time.deltaTime;
            if (timerWard <= 0)
            {
                GameObject w = Instantiate(ward);
                w.transform.position = transform.position;
                FogRevealer fogRevealer = new FogRevealer(w.transform, 10, false);
                GameManager.Instance.fogWar._FogRevealers.Add(fogRevealer);
                timerWard = timerWardMax;
            }
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (state == State.ENEMY)
        {
            navMeshAgent.enabled = false;
            transform.position += speed * directionEnemy * Time.deltaTime;
            PlayAnimation("Run");
        }
        
        //Enemy Detection Sphere
        if (!checkEnemy)
        {
            Vector3 boxCenter = transform.position + transform.forward * 3;
            Vector3 boxSize = Vector3.one * flankRange;
            Quaternion boxOrientation = transform.rotation;
        
            //Enemy Detection Forward Box
            RaycastHit[] hitsBox = Physics.BoxCastAll(boxCenter, boxSize, transform.forward, boxOrientation, layerMaskTroop);
        
            foreach (var hit in hitsBox)
            {
                Troop enemyTroop = null;
                if (hit.transform.gameObject.TryGetComponent(out enemyTroop))
                {
                    if (!hit.transform.gameObject.CompareTag(gameObject.tag))
                    {
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);
                        // Check flank 
                        TargetEnemyFlank(Vector3.Angle(enemyTroop.transform.forward, transform.forward));
                        enemyTroop.GetTargeted(this);
                        checkEnemy = true;
                        break;
                    }
                }
            }
            DebugDrawBox(boxCenter, boxSize, boxOrientation);
            
            RaycastHit[] hitsSphere = Physics.SphereCastAll(transform.position, aoeRange / 2, transform.forward, 10000, layerMaskTroop);
        
            foreach (var hit in hitsSphere)
            {
                Troop enemyTroop = null;
                if (hit.transform.gameObject.TryGetComponent(out enemyTroop))
                {
                    if (!hit.transform.gameObject.CompareTag(gameObject.tag))
                    {
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);
                        GiveTarget();
                        checkEnemy = true;
                    }
                }
            }
        }
    }

    private void TargetEnemyFlank(float angleFlank)
    {
        FlankValues value = angleFlank switch
        {
            <= 45 => FlankValues.BACK,
            <= 135 => FlankValues.SIDES,
            >= 136 => FlankValues.FRONT,
        };
        Debug.LogError(angleFlank + " " + value);
        GiveTarget(value);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aoeRange / 2);
    }
    
    
    void DebugDrawBox(Vector3 position, Vector3 size, Quaternion orientation, float duration = 0.2f)
    {
        Vector3 halfSize = size / 2;

        // Calculer les 8 coins de la box
        Vector3[] corners = new Vector3[8]
        {
            position + orientation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            position + orientation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            position + orientation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            position + orientation * new Vector3(-halfSize.x, halfSize.y, halfSize.z),
            position + orientation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            position + orientation * new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            position + orientation * new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            position + orientation * new Vector3(halfSize.x, halfSize.y, halfSize.z)
        };

        // Dessiner les 12 arêtes de la box
        Debug.DrawLine(corners[0], corners[1], Color.green, duration);
        Debug.DrawLine(corners[0], corners[2], Color.green, duration);
        Debug.DrawLine(corners[0], corners[4], Color.green, duration);
        Debug.DrawLine(corners[1], corners[3], Color.green, duration);
        Debug.DrawLine(corners[1], corners[5], Color.green, duration);
        Debug.DrawLine(corners[2], corners[3], Color.green, duration);
        Debug.DrawLine(corners[2], corners[6], Color.green, duration);
        Debug.DrawLine(corners[3], corners[7], Color.green, duration);
        Debug.DrawLine(corners[4], corners[5], Color.green, duration);
        Debug.DrawLine(corners[4], corners[6], Color.green, duration);
        Debug.DrawLine(corners[5], corners[7], Color.green, duration);
        Debug.DrawLine(corners[6], corners[7], Color.green, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        // if (checkEnemy) return;
        //
        // if ((other.tag == "Enemy" && tag == "Player") || (other.tag == "Player" && tag == "Enemy"))
        // {
        // state = State.RUNATTACK;
        // PlayAnimation("Run");
        // target = other.gameObject.GetComponent<Entity>();
        // GiveTarget();
        // checkEnemy = true;
        // }
    }
}
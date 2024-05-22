using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
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

    //Waypoints
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
    private Vector3 lastPositionMove;
    Mark currentMark;
    LineRenderer currentLine;
    private bool canRun = true;

    //STATS
    private float speed;
    private float range;
    private int aoeRange;
    private int flankRange;
    private float timerAttackMax;
    private float timerAttack = 0f;
    public int maxTroop;

    [SerializeField] private LayerMask layerMaskTroopTarget; 
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    //Constructible Capture
    public Constructible areaToCapture;

    public bool hitByHouse;


    //FOGWAR
    [SerializeField] GameObject ward;
    private float timerWardMax = 2f;
    private float timerWard;
    public FogRevealer fogRevealer;

    private GameManager gameManager;
    
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    
    public override void Start()
    {
        base.Start();

        gameManager = GameManager.Instance;

        //STATS UNIT FOR TROOPS NOT INSTANCED
        if (L_Units.Count > 0)
        {
            unitRef = L_Units[0];
        }

        //ADD FOG REVEAL
        if (gameManager.CheckIsVicars(owner))
        {
            fogRevealer = new FogRevealer(transform, 10, true);
            gameManager.fogWar._FogRevealers.Add(fogRevealer);
        }

        speed = unitRef.speed;
        navMeshAgent.speed = speed;
        range = unitRef.range;
        aoeRange = unitRef.aoeRange;
        flankRange = unitRef.flankRange;
        timerAttackMax = unitRef.timerAttackMax;
        maxTroop = unitRef.maxTroop;

        state = State.WAITING;
        lastPositionMove = transform.position;

        if (!gameManager.CheckIsVicars(owner))
        {
            state = State.ENEMY;
            directionEnemy = transform.forward;
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
        if (!gameManager.CheckIsVicars(owner)) return;

        foreach (EntityUnit unit in L_Units)
        {
            if (unit.outline)
            {
                unit.outline.OutlineMode = Outline.Mode.OutlineAll;
                unit.outline.OutlineWidth = 0.5f;
            }
        }
    }

    public override void selectOutline()
    {
        if (!gameManager.CheckIsVicars(owner)) return;

        foreach (EntityUnit unit in L_Units)
        {
            if (unit.outline)
            {
                unit.outline.OutlineMode = Outline.Mode.OutlineAll;
                unit.outline.OutlineWidth = 1.3f;
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

                if (messenger.canMsg)
                {
                    messenger.ChooseTroop();
                    gameManager.ChangeCursor(gameManager.cursorMsg);
                }
            }
        }

        if (!myWayPoints && canRun && type != Type.Messenger)
        {
            gameManager.CreatePath();
        }
        
        //TODO Display UI
        if (type != Type.Messenger)
        { 
            UIManager.Instance.DisplayTroopInfos(this, true);
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
                gameManager.ChangeCursor(gameManager.cursorNormal);
            }
        }
        
        UIManager.Instance.DisplayTroopInfos(this, false);
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
            float multiValue = 0f;
            switch (flankValue)
            {
                case FlankValues.BACK :
                    multiValue = 2f;
                    break;
                case FlankValues.FRONT :
                    multiValue = 1f;
                    break;
                case FlankValues.SIDES :
                    multiValue = 1.5f;
                    break;
            }
            unit.AddBonusDmgFlank(multiValue);
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
        if (!target.TryGetComponent<Hero>(out Hero hero)) Destroy(target.gameObject);

        target = null;
        foreach (EntityUnit unit in L_Units)
        {
            unit.target = null;
            unit.ResetBonusDmg();
        }
    }

    public void Run()
    {
        state = State.RUNNING;
        currentMark = myWayPoints.marks[0];
        navMeshAgent.SetDestination(currentMark.transform.position);
        // transform.position = currentMark.transform.position;
        currentMark.gameObject.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
        //transform.LookAt(currentMark.transform);
        if (myHouse) myHouse.DetachTroop(this);

    }

    public void RemoveUnit(EntityUnit unit)
    {
        L_Units.Remove(unit);
        if (L_Units.Count <= 0)
        {
            if (fogRevealer != null) gameManager.fogWar._FogRevealers.Remove(fogRevealer);
            if (myWayPoints) 
            {
                gameManager.L_WayPoints.Remove(myWayPoints);
                Destroy(myWayPoints.gameObject);
            }
            
            if (areaToCapture)
            {
                if (gameManager.CheckIsVicars(owner))
                {
                    areaToCapture.playerCapturing = false;
                }
                else
                {
                    areaToCapture.enemyCapturing = false;
                }
            }
            
            Destroy(gameObject);
        }
    }

    public void UpdateSpeedTroop(float newSpeed)
    {
        speed = newSpeed; 
        navMeshAgent.speed = newSpeed;
    }

    private void Update()
    {
        /////////////////  DO SOMETHING IF SELECTED ////////////////////
        if (gameManager.selectedUnit == this)
        {
            if (type != Type.Messenger)
            {
                /////////////////////// GO //////////////////////////
                if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space)) && myWayPoints && !gameManager.isPathing && state == State.WAITING)
                {
                    Run();
                    canRun = false;
                }
            }
            else
            {
                if (L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                {
                    ////////////////////// BRING MESSAGE /////////////////////
                    if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space)) && messenger.canGo)
                    {
                        messenger.Go();
                        gameManager.ChangeCursor(gameManager.cursorNormal);
                    }
                }
            }
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward) * 3;
        Debug.DrawRay(transform.position, forward, Color.red);
        //////////////////   STATE   ////////////////////////////////////
        if (state != State.SLEEPING)
        {
            if (gameManager.CheckIsVicars(owner) && currentLine)
            {
                currentLine.SetPosition(0, RayTheFloor(gameManager.layer_mask));
            }

            if (state == State.RUNNING && myWayPoints)
            {
                PlayAnimation("Run");
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(currentMark.transform.position);
                // transform.position = Vector3.MoveTowards(transform.position, currentMark.transform.position, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.4f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
                {
                    currentMark.gameObject.SetActive(false);
                    currentMark = myWayPoints.nextPoint(currentMark);
                    currentLine.gameObject.SetActive(false);
                    currentLine = myWayPoints.nextLine(currentLine);
                    // transform.LookAt(currentMark.transform);
                }
                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.4f && myWayPoints.marks.IndexOf(currentMark) == myWayPoints.marks.Count - 1)
                {
                    gameManager.L_WayPoints.Remove(myWayPoints);
                    Destroy(myWayPoints.gameObject);
                    state = State.WAITING;
                }

                // Save last position before changing state
                lastPositionMove = transform.position;
            }

            if (state == State.WAITING && type != Type.Messenger)
            {
                if (Vector3.Distance(transform.position, lastPositionMove) >= 1)
                {
                    PlayAnimation("Run");
                    navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(lastPositionMove);

                    if (hitByHouse)
                    {
                        lastPositionMove = transform.position;
                        hitByHouse = false;
                    }
                }
                else
                {
                    PlayAnimation("Idle");
                }
            }

            //////////////// FIGHTING  ////////////////
            if (state == State.RUNATTACK)
            {
                if (target == null)
                {
                    if (myWayPoints) state = State.RUNNING;
                    else state = State.WAITING;
                    checkEnemy = false;

                    if (!gameManager.CheckIsVicars(owner))
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
                        if (!gameManager.CheckIsVicars(owner))
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
                    target.Die();
                    //DEATH
                    KillTarget();

                    state = State.RUNATTACK;

                    if (!gameManager.CheckIsVicars(owner))
                    {
                        state = State.ENEMY;
                        transform.LookAt(directionEnemy);
                        checkEnemy = false;
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
                GameObject w = Instantiate(ward, gameManager.fogWar.transform);
                w.transform.position = transform.position;
                FogRevealer fogRevealer = new FogRevealer(w.transform, 10, false);
                gameManager.fogWar._FogRevealers.Add(fogRevealer);
                timerWard = timerWardMax;
            }
        }

        //////////////////   ENEMY   ////////////////////////////////////
        if (state == State.ENEMY)
        {
            //navMeshAgent.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, gameManager.basePlayer.transform.position, speed * Time.deltaTime);
            transform.LookAt(gameManager.basePlayer.transform.position);
            PlayAnimation("Run");
        }

        //Enemy Detection Sphere
        if (!checkEnemy && type != Type.Messenger)
        {
            Vector3 boxCenter = transform.position + transform.forward * 3;
            Vector3 boxSize = Vector3.one * flankRange;
            Quaternion boxOrientation = transform.rotation;

            //Enemy Detection Forward Box
            RaycastHit[] hitsBox = Physics.BoxCastAll(boxCenter, boxSize, transform.forward, boxOrientation, 3, layerMaskTroopTarget);

            foreach (var hit in hitsBox)
            {
                Troop enemyTroop = null;
                if (hit.transform.gameObject.TryGetComponent(out enemyTroop))
                {
                    if (enemyTroop.owner != owner && enemyTroop.type != Type.Messenger)
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
            Vector3[] cubePoint = CubePoints(boxCenter, boxSize, boxOrientation);
            DrawCubePoints(cubePoint);

            RaycastHit[] hitsSphere = Physics.SphereCastAll(transform.position, aoeRange / 2, transform.up, 10000, layerMaskTroopTarget);

            foreach (var hit in hitsSphere)
            {
                if (hit.transform.gameObject.TryGetComponent(out Troop enemyTroop))
                {
                    if (enemyTroop.owner != owner && enemyTroop.type != Type.Messenger)
                    {
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);

                    }
                }

                if (hit.transform.gameObject.TryGetComponent(out EntityHouse enemyBuilding))
                {
                    if (enemyBuilding.House.owner != owner)
                    {
                        target = enemyBuilding;
                    }
                }

                if (target != null)
                {
                    state = State.RUNATTACK;
                    PlayAnimation("Run");
                    GiveTarget();
                    checkEnemy = true;
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
        GiveTarget(value);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aoeRange / 2);
    }

    Vector3[] CubePoints(Vector3 center, Vector3 extents, Quaternion rotation)
    {
        Vector3[] points = new Vector3[8];
        points[0] = rotation * Vector3.Scale(extents, new Vector3(1, 1, 1)) + center;
        points[1] = rotation * Vector3.Scale(extents, new Vector3(1, 1, -1)) + center;
        points[2] = rotation * Vector3.Scale(extents, new Vector3(1, -1, 1)) + center;
        points[3] = rotation * Vector3.Scale(extents, new Vector3(1, -1, -1)) + center;
        points[4] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, 1)) + center;
        points[5] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, -1)) + center;
        points[6] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, 1)) + center;
        points[7] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, -1)) + center;

        return points;
    }

    void DrawCubePoints(Vector3[] points, float duration = 0.2f)
    {
        Debug.DrawLine(points[0], points[1], Color.green, duration);
        Debug.DrawLine(points[0], points[2], Color.green, duration);
        Debug.DrawLine(points[0], points[4], Color.green, duration);

        Debug.DrawLine(points[7], points[6], Color.green, duration);
        Debug.DrawLine(points[7], points[5], Color.green, duration);
        Debug.DrawLine(points[7], points[3], Color.green, duration);

        Debug.DrawLine(points[1], points[3], Color.green, duration);
        Debug.DrawLine(points[1], points[5], Color.green, duration);

        Debug.DrawLine(points[2], points[3], Color.green, duration);
        Debug.DrawLine(points[2], points[6], Color.green, duration);

        Debug.DrawLine(points[4], points[5], Color.green, duration);
        Debug.DrawLine(points[4], points[6], Color.green, duration);
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
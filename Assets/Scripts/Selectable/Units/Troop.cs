using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    [Header("Base Troop")]
    public List<EntityUnit> L_Units = new List<EntityUnit>();
    private EntityUnit unitRef;
    protected float radius = 0.8f;
    public Type type;
    public State state;
    public bool checkEnemy;
    public bool checkBuilding;
    public bool isOver;
    protected Entity target;
    private Troop currentFightingTroop;
    private FlankValues currentFlank;
    private 
    Vector3 directionEnemy;
    public House myHouse;
    public Color healthBarColor;
    public HealthBar healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    [Header("WayPoints")]
    public WayPoints myWayPoints;
    public WayPoints changingWayPoints;
    protected Vector3 lastPositionMove;
    Mark currentMark;
    LineRenderer currentLine;
    float lastClickTime;
    private bool isRunning;
    public bool isWaypoints;

    [Header("Stats")]
    protected float speed;
    protected float range;
    protected int aoeRange;
    protected int flankRange;
    protected float timerAttackMax;
    protected float timerAttack = 0f;
    public int maxTroop;
    public UITroopInfo uiInfos;

    [SerializeField] protected LayerMask layerMaskTroopTarget; 
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    [Header("Constructible Capture")]
    public Constructible areaToCapture;

    public bool hitByHouse;


    [Header("Fogwar")]
    [SerializeField] GameObject ward;
    private float timerWardMax = 2f;
    private float timerWard;
    public FogRevealer fogRevealer;

    protected GameManager gameManager;
    
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

        //Health Bar
        healthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBar.StartBar(gameObject, healthBarColor);
        UpdateHealthBarTroop();
    }

    public IEnumerator AddUnit(EntityUnit unit)
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
        yield return new WaitUntil(() => healthBar != null);
        UpdateHealthBarTroop();
    }

    public void UpdateHealthBarTroop()
    {
        //Refresh for each unit life
        if (L_Units.Count > 0)
        {
            float totalValueLife = 0;
            foreach (var unit in L_Units)
            {
                totalValueLife += (float)unit.PV / (float)unit.maxPV;
            }
            healthBar.slider.value = totalValueLife / (float)L_Units.Count;
        }
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

    public void ResetTroop()
    {
        state = State.WAITING;
        
        if (myWayPoints) 
        {
            gameManager.L_WayPoints.Remove(myWayPoints);
            Destroy(myWayPoints.gameObject);
            myWayPoints = null;
        }
    }

    public void ResetNavMesh()
    {
        lastPositionMove = transform.position;
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
    }

    public override void Select()
    {
        base.Select();

        if (myWayPoints) myWayPoints.gameObject.SetActive(true);
        if (changingWayPoints) changingWayPoints.gameObject.SetActive(true);

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

        if (!myWayPoints && type != Type.Messenger && !isWaypoints)
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
        if (changingWayPoints) changingWayPoints.gameObject.SetActive(false);

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

    protected void PlayAnimation(string name)
    {
        foreach (EntityUnit unit in L_Units)
        {
            if (unit.animator.isActiveAndEnabled) unit.animator.Play(name);
        }
    }

    protected void Attack()
    {
        foreach (EntityUnit unit in L_Units)
        {
            unit.Attack();
        }
    }

    protected void GiveTarget(FlankValues flankValue = FlankValues.FRONT)
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
            healthBar.Bonus(flankValue);
            unit.AddBonusDmgFlank(multiValue);
        }
    }

    private void NoBonus()
    {
        foreach (EntityUnit unit in L_Units)
        {
            healthBar.Bonus(FlankValues.FRONT);
        }
    }

    public EntityUnit GetNearestUnitFromTroop(Vector3 pos)
    {
        EntityUnit nearestUnit = null;
        foreach (EntityUnit unit in L_Units)
        {
            if (!unit.isActiveAndEnabled) continue;
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

    public void GetTargeted(Troop enemyTroop)
    {
        if (target == null)
        {
            target = enemyTroop.GetNearestUnitFromTroop(transform.position);
            checkEnemy = true;
            GiveTarget();
            state = State.RUNATTACK;
        }
    }

    protected void KillTarget()
    {
        //if (!target.TryGetComponent<Hero>(out Hero hero)) Destroy(target.gameObject);
        target = null;
        foreach (EntityUnit unit in L_Units)
        {
            unit.target = null;
            unit.ResetBonusDmg();
        }
    }

    public void Run()
    {
        PlayAnimation("Run");
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        state = State.RUNNING;
        currentMark = myWayPoints.marks[0];
        currentMark.gameObject.SetActive(false);
        currentMark = myWayPoints.nextPoint(currentMark);
        currentLine = myWayPoints.lines[0];
        navMeshAgent.SetDestination(currentMark.transform.position);

        if (myHouse) myHouse.DetachTroop(this);

        if (uiInfos) uiInfos.btnGo.interactable = false;
    }

    public void RemoveUnit(EntityUnit unit)
    {
        L_Units.Remove(unit);
        CheckTroopIsEmpty();
    }

    private void CheckTroopIsEmpty()
    {
        if (L_Units.Find(unit => unit.PV > 0) == null)
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
            Destroy(healthBar.gameObject);

            Destroy(gameObject);
        }
    }

    public void DisableUnit(EntityUnit unit)
    {
        //TODO Faire une animation de mort
        unit.gameObject.SetActive(false);
        CheckTroopIsEmpty();
        
        UpdateHealthBarTroop();
    }

    public void UpdateSpeedTroop(float newSpeed)
    {
        speed = newSpeed; 
        navMeshAgent.speed = newSpeed;
    }

    private void GoTo(Vector3 pos)
    {
        PlayAnimation("Run");
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(pos);
        isRunning = true;
    }

     protected virtual void Update()
    {
        /////////////////  DO SOMETHING IF SELECTED ////////////////////
        if (gameManager.selectedUnit == this)
        {
            if (type != Type.Messenger && !gameManager.isPathing && state == State.WAITING)
            {
                /////////////////////// GO //////////////////////////
                                //DOUBLECLICK
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    float timeSinceLastClick = Time.unscaledTime - lastClickTime;

                    if (timeSinceLastClick <= gameManager.DOUBLE_CLICK_TIME * Time.timeScale)
                    {
                        Run();
                        isWaypoints = true;
                    }

                    lastClickTime = Time.unscaledTime;
                }
            }
            else
            {
                if (L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                {
                    ////////////////////// BRING MESSAGE /////////////////////
                    if (Input.GetKeyDown(KeyCode.Space) && messenger.canGo)
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
                if (!isRunning) GoTo(currentMark.transform.position);

                if (Vector3.Distance(transform.position, currentMark.transform.position) < 0.4f && myWayPoints.marks.IndexOf(currentMark) < myWayPoints.marks.Count - 1)
                {
                    currentMark.gameObject.SetActive(false);
                    currentMark = myWayPoints.nextPoint(currentMark);
                    currentLine.gameObject.SetActive(false);
                    currentLine = myWayPoints.nextLine(currentLine);
                    isRunning = false;
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
                    if (!isRunning) GoTo(lastPositionMove);

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
                    isRunning = false;
                    if (myWayPoints && currentMark) state = State.RUNNING;
                    else state = State.WAITING;
                    checkEnemy = false;
                    checkBuilding = false;
                    NoBonus();
                    if (!gameManager.CheckIsVicars(owner))
                    {
                        state = State.ENEMY;
                        transform.LookAt(directionEnemy);
                    }
                }
                else
                {
                    Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    if (!isRunning) GoTo(targetPos);
                    if (Vector3.Distance(transform.position, target.transform.position) <= range + target.size)
                    {
                        state = State.ATTACK;
                        navMeshAgent.isStopped = true;
                        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 15f)
                    {
                        state = State.RUNNING;
                        navMeshAgent.isStopped = false;
                        checkEnemy = false;
                        checkBuilding = false;
                        isRunning = false;
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
                    timerAttack = 0f;
                    state = State.RUNATTACK;
                    if (!gameManager.CheckIsVicars(owner))
                    {
                        state = State.ENEMY;
                        transform.LookAt(directionEnemy);
                        checkEnemy = false;
                        checkBuilding = false;
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
                    if (Vector3.Distance(transform.position, target.transform.position) > 1.5f * range + target.size)
                    {
                        timerAttack = 0f;
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
        
        //ENEMY DETECTION
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
                        isRunning = false;
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);
                        // Check flank 
                        if (enemyTroop == currentFightingTroop)
                        {
                            GiveTarget(currentFlank);
                        }
                        else TargetEnemyFlank(Vector3.Angle(enemyTroop.transform.forward, transform.forward));
                        currentFightingTroop = enemyTroop;
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
                        isRunning = false;
                        timerAttack = 0f;
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);
                        checkEnemy = true;
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        if (enemyTroop == currentFightingTroop)
                        {
                            GiveTarget(currentFlank);
                        }
                        else GiveTarget();
                    }
                }

                if (hit.transform.gameObject.TryGetComponent(out EntityHouse enemyBuilding))
                {
                    if (!checkBuilding && enemyBuilding.House.owner != owner)
                    {
                        target = enemyBuilding;
                        checkBuilding = true;
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        GiveTarget();
                    }
                }
            }
        }
    }

     protected void TargetEnemyFlank(float angleFlank)
    {
        FlankValues value = angleFlank switch
        {
            <= 45 => FlankValues.BACK,
            <= 135 => FlankValues.SIDES,
            >= 136 => FlankValues.FRONT,
        };
        currentFlank = value;
        GiveTarget(value);
    }

     protected void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aoeRange / 2);
    }

     protected Vector3[] CubePoints(Vector3 center, Vector3 extents, Quaternion rotation)
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

     protected void DrawCubePoints(Vector3[] points, float duration = 0.2f)
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
}
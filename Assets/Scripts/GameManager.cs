using FischlWorks_FogWar;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerBaseSetup
{
    [SerializeField] private Player player;
    [SerializeField] private List<EntityUnit> L_BaseUnit;
    [SerializeField] private List<House> L_BaseBuildings;
    //TODO Add Hero and companions

    public Player Player { get => player; }
    public List<EntityUnit> BaseUnit { get => L_BaseUnit; }
    public List<House> BaseBuildings { get => L_BaseBuildings; }
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game System")]
    public bool isPause;
    public bool inGamePause;
    public csFogWar fogWar;
    public float timerGame;
    [Header("Team System")]
    [SerializeField] private PlayerBaseSetup vicarPlayer;
    [SerializeField] private PlayerBaseSetup vikingPlayer;
    
    [Header("Pathing System")]
    [SerializeField] WayPoints wayPoints;
    public WayPoints currentWayPoints;
    public List<WayPoints> L_WayPoints = new List<WayPoints>();
    private bool isAllWayPoints;

    [SerializeField] Mark mark;
    public Mark currentMark;

    public bool isPathing;

    [SerializeField] LineRenderer _lr;
    LineRenderer currentLine;


    [Header("Pathing System")]
    [SerializeField] GameObject floor;
    public int layer_mask;
    public int layer_mark;

    public Selectable selectedUnit;

    public bool isBuilding = false;
    public House houseToBuild;

    [Header("UI System")]
    [SerializeField] Canvas mainMenu;
    TextMeshProUGUI textGold;
    TextMeshProUGUI textReputation;

    public HoverTitle objectHover;
    public GameObject panelHover;

    public bool isFirstMessage;

    [Header("Resources")]
    public int gold;
    public int reputation;

    //Goal System
    [HideInInspector] public UnityEvent<Player> onBaseIsDestroyed = new UnityEvent<Player>();
    [HideInInspector] public UnityEvent<Player> onHeroIsKilled = new UnityEvent<Player>();
    [HideInInspector] public UnityEvent<float> onTimeIsUp = new UnityEvent<float>();
    [HideInInspector] public UnityEvent<Player> onTreasureIsSecured = new UnityEvent<Player>();

    [Header("Cursor")]
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public Texture2D cursorMsg;
    public Texture2D cursorNormal;
    public Texture2D cursorLeft;
    public Texture2D cursorRight;
    public Texture2D cursorUp;
    public Texture2D cursorDown;
    private UIManager uiManager;

    public GameObject basePlayer;


    public PlayerBaseSetup VicarPlayer { get => vicarPlayer; }

    public PlayerBaseSetup VikingPlayer { get => vikingPlayer; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeCursor(cursorNormal);

        layer_mask = LayerMask.GetMask("Floor");
        layer_mark = LayerMask.GetMask("Mark");
        StopBuilding();

        textGold = mainMenu.gameObject.transform.Find("Ressources").Find("Gold").Find("Text").GetComponent<TextMeshProUGUI>();
        textReputation = mainMenu.gameObject.transform.Find("Ressources").Find("Reputation").Find("Text").GetComponent<TextMeshProUGUI>();
        UpdateRessources();

        panelHover = mainMenu.gameObject.transform.Find("Hover").gameObject;
        panelHover.SetActive(false);
        
        //Setup Players
        Player vic_Player = vicarPlayer.Player;
        // Vicars Goals
        vic_Player.GameSetup.GameGoal.SetupCheckGoalDone(vic_Player);
        // Vicars Units
        // TODO Faire spawn les unités dès le début
        // Vicars Buildings
        foreach (var building in vicarPlayer.BaseBuildings)
        {
            building.owner = vic_Player;
        }
        // Vikings
        Player vik_Player = vikingPlayer.Player;
        vik_Player.GameSetup.GameGoal.SetupCheckGoalDone(vik_Player);
        
        foreach (var building in vikingPlayer.BaseBuildings)
        {
            building.owner = vik_Player;
        }
        
        uiManager = UIManager.Instance;
        
        // Reset Timer
        timerGame = 0;
        InvokeRepeating("CheckEachSeconds", 0f, 1f);
    }


    public PlayerBaseSetup GetPlayerBaseSetup(Player player)
    {
        return vicarPlayer.Player == player ? vicarPlayer : vikingPlayer;
    }

    #region Pathing
    
    public void ChangeCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, hotSpot, cursorMode);
    }

    public void CreatePath()
    {
        if (!isPathing)
        {
            isPathing = true;
            currentWayPoints = Instantiate(wayPoints);
            L_WayPoints.Add(currentWayPoints);

            Mark firstMark = Instantiate(mark, currentWayPoints.transform);
            firstMark.transform.position = selectedUnit.RayTheFloor(layer_mask);

            currentWayPoints.AddMark(firstMark);
            currentMark = Instantiate(mark, currentWayPoints.transform);
            currentWayPoints.AddMark(currentMark);
            currentWayPoints.lineColor = Color.red;
            CreateLine();
            isFirstMessage = true;
        }   
    }
    public void CreateNewPath()
    {
        if (!isPathing)
        {
            isPathing = true;
            currentWayPoints = Instantiate(wayPoints);
            L_WayPoints.Add(currentWayPoints);

            Mark firstMark = Instantiate(mark, currentWayPoints.transform);
            if (selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
            {
                if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                {
                    firstMark.transform.position = messenger.troopSelected.RayTheFloor(layer_mask);
                    currentWayPoints.AddMark(firstMark);
                    currentMark = Instantiate(mark, currentWayPoints.transform);
                    currentWayPoints.AddMark(currentMark);
                    currentWayPoints.lineColor = Color.blue;
                    currentWayPoints.isNew = true;
                    CreateLine();
                    isFirstMessage = true;
                }
                else
                {
                    Debug.LogWarning("No Messegner");
                }
            }
            else
            {
                Debug.LogWarning("No Troop");
            }
        }
    }

    void CreateLine()
    {
        int index = currentWayPoints.marks.IndexOf(currentMark);
        currentLine = Instantiate(_lr, currentWayPoints.transform);
        currentLine.material.color = currentWayPoints.lineColor;
        currentLine.SetPosition(0, currentWayPoints.marks[index - 1].transform.position);
        currentLine.SetPosition(1, currentMark.transform.position);
        currentLine.startWidth = currentWayPoints.lineWidth;
        currentLine.endWidth = currentWayPoints.lineWidth;
        currentWayPoints.lines.Add(currentLine);
    }
    
    #endregion

    #region GoalSystem

    public void AddListenerTreasureGoal(UnityAction<Player> action)
    {
        onTreasureIsSecured.AddListener(action);
    }

    public void AddListenerHeroGoal(UnityAction<Player> action)
    {
        onHeroIsKilled.AddListener(action);
    }
    
    public void AddListenerBaseGoal(UnityAction<Player> action)
    {
        onBaseIsDestroyed.AddListener(action);
    }
    
    public void AddListenerTimeRemainGoal(UnityAction<float> action)
    {
        onTimeIsUp.AddListener(action);
    }

    public void PlayerWinGame(Player playerRef)
    {
        if (playerRef == vicarPlayer.Player)
        {
            UIManager.Instance.Victory();
        }
        else
        {
            UIManager.Instance.Defeat();
        }
    }
    
    #endregion

    private void Update()
    {
        /////////////////// RAYCAST CURRENT MARK ////////////////////////
        if (isPathing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layer_mask))
            {
                currentMark.transform.position = hit.point;
                currentLine.SetPosition(1, hit.point);
            }
        }

        // TIMER GAME
        timerGame += Time.deltaTime;
        uiManager.UpdateTimer(timerGame);
        
        /////////////////////////////// PATH WAYPOINTS UNIT /////////////////////////////////////
        if (isPathing && Input.GetMouseButtonDown(0) && !isFirstMessage && currentMark.canBuild)
        {
            Vector3 currentPos = currentMark.transform.position;
            currentMark = Instantiate(mark, currentWayPoints.transform);
            currentWayPoints.AddMark(currentMark);
            currentMark.transform.position = currentPos;
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {
            isFirstMessage = false;
        }

        //////// STOP PATHING
        if (isPathing && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1)))
        {
            if (currentWayPoints.marks.Count <= 2)
            {
                L_WayPoints.Remove(currentWayPoints);
                Destroy(currentWayPoints.gameObject);
                if (selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
                {
                    if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger msg))
                    {
                        msg.Reset();
                    }
                }
            }
            else
            {
                currentWayPoints.marks.Remove(currentMark);
                DestroyImmediate(currentMark.gameObject);
                DestroyImmediate(currentLine.gameObject);
                if (selectedUnit.TryGetComponent<Troop>(out Troop troop))
                {
                    if (troop.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                    {
                        messenger.canGo = true;
                    }
                }
            }
            isPathing = false;
        }
        ////// GAME PAUSE
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.Pause();
        }
        ////// UNSELECT UNIT
        else if (Input.GetMouseButtonDown(1))
        {
            if (selectedUnit) selectedUnit.UnSelect();
            selectedUnit = null;
        }

        /////////////////////////////// BUILDING /////////////////////////////////////
        if (isBuilding)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBuilding();
            }
        }

        /////////////////////////////// CHEAT CODE /////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.L))
        {
            reputation += 100;
            gold += 100;
            UpdateRessources();
        }

        ////////////////////////////// PAUSE IN GAME //////////////////////////////////////////
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!inGamePause) 
            {
                inGamePause = true;
                Time.timeScale = 0;
            }
            else
            {
                inGamePause = false;
                Time.timeScale = 1;
            }
        }

        ////////////////////////////// SHOW / HIDE ALL WAYPOINTS //////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isAllWayPoints)
            {
                isAllWayPoints = true;
                foreach (WayPoints wayPoints in L_WayPoints)
                {
                    wayPoints.gameObject.SetActive(true);
                }
            }
            else
            {
                isAllWayPoints = false;
                foreach (WayPoints wayPoints in L_WayPoints)
                {
                    wayPoints.gameObject.SetActive(false);
                }
            }
        }
    }

    public void StopBuilding()
    {
        isBuilding = false;
        houseToBuild = null;
    }

    public void UpdateRessources()
    {
        textGold.text = gold.ToString();
        textReputation.text = reputation.ToString();
    }

    private void CheckEachSeconds()
    {
        onTimeIsUp.Invoke(timerGame);
    }

    public bool CheckIsVicars(Player playerRef)
    {
        return playerRef == vicarPlayer.Player;
    }
}

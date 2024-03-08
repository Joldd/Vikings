using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] WayPoints wayPoints;
    public WayPoints currentWayPoints;

    [SerializeField] GameObject mark;
    public GameObject currentMark;

    public bool isPathing;

    [SerializeField] LineRenderer _lr;
    LineRenderer currentLine;
    [SerializeField] float lineWidth = 0.2f;

    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject floor;
    int layer_mask;

    public Selectable selectedUnit;

    public bool isBuilding = false;
    public House houseToBuild;

    public GameObject buildings;
    public GameObject spawnsBuildings;


    [SerializeField] Canvas mainMenu;
    TextMeshProUGUI textGold;
    TextMeshProUGUI textReputation;
    public int gold;
    public int reputation;

    public HoverTitle objectHover;
    public GameObject panelHover;

    public bool isChoosingMessager;
    public bool isFirstMessage; 

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
       layer_mask = LayerMask.GetMask("Floor");
        StopBuilding();

        textGold = mainMenu.gameObject.transform.Find("Ressources").Find("Gold").Find("Text").GetComponent<TextMeshProUGUI>();
        textReputation = mainMenu.gameObject.transform.Find("Ressources").Find("Reputation").Find("Text").GetComponent<TextMeshProUGUI>();
        reputation = 0;
        gold = 0;
        updateRessources();

        panelHover = mainMenu.gameObject.transform.Find("Hover").gameObject;
        panelHover.SetActive(false);
    }

    public void createPath()
    {
        if (!isPathing)
        {
            isPathing = true;
            currentWayPoints = Instantiate(wayPoints);
            GameObject firstMark = Instantiate(mark, currentWayPoints.transform);
            firstMark.transform.position = selectedUnit.transform.position;
            currentWayPoints.marks.Add(firstMark);
            currentMark = Instantiate(mark, currentWayPoints.transform);
            currentWayPoints.marks.Add(currentMark);
            currentWayPoints.lineColor = Color.red;
            createLine();
        }   
    }

    public void createNewPath()
    {
        if (!isPathing)
        {
            
            isPathing = true;
            currentWayPoints = Instantiate(wayPoints);
            GameObject firstMark = Instantiate(mark, currentWayPoints.transform);
            Messenger messenger = selectedUnit.GetComponent<Messenger>();
            firstMark.transform.position = messenger.vikingSelected.transform.position;
            currentWayPoints.marks.Add(firstMark);
            currentMark = Instantiate(mark, currentWayPoints.transform);
            currentWayPoints.marks.Add(currentMark);
            currentWayPoints.lineColor = Color.blue;
            currentWayPoints.isNew = true;
            createLine();
            isFirstMessage = true;
        }
    }

    void createLine()
    {
        int index = currentWayPoints.marks.IndexOf(currentMark);
        currentLine = Instantiate(_lr, currentWayPoints.transform);
        currentLine.material.color = currentWayPoints.lineColor;
        currentLine.SetPosition(0, currentWayPoints.marks[index - 1].transform.position);
        currentLine.SetPosition(1, currentMark.transform.position);
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentWayPoints.lines.Add(currentLine);
    }

    public GameObject createUnit(GameObject unit, Transform spawn)
    {
        if (gold >= unit.GetComponent<Selectable>().priceGold)
        {
            GameObject v = Instantiate(unit);
            v.transform.position = spawn.transform.position;
            gold -= unit.GetComponent<Selectable>().priceGold;
            updateRessources();
            return v;
        }
        else
        {
            return null;
        }
    }

    private void FixedUpdate()
    {
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
    }

    private void Update()
    {

        /////////////////////////////// PATH WAYPOINTS UNIT /////////////////////////////////////
        if (isPathing && Input.GetMouseButtonDown(0) && !isFirstMessage)
        {
            Vector3 currentPos = currentMark.transform.position;
            currentMark = Instantiate(mark, currentWayPoints.transform);
            currentWayPoints.marks.Add(currentMark);
            currentMark.transform.position = currentPos;
            createLine();
        }

        if (Input.GetMouseButton(0))
        {
            isFirstMessage = false;
        }

        if (isPathing && Input.GetKeyDown(KeyCode.Escape))
        {
            currentWayPoints.marks.Remove(currentMark);
            DestroyImmediate(currentMark);
            DestroyImmediate(currentLine);
            isPathing = false;
            if (selectedUnit.TryGetComponent<Viking>(out Viking v))
            {
                v.btnDraw.interactable = false;
                v.btnRun.interactable = true;
            }
        }

        /////////////////////////////// BUILDING /////////////////////////////////////
        if (isBuilding)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBuilding();
            }
        }


        //////////////////////////////// HOVER UI ///////////////////////////////////////


        /////////////////////////////// CHEAT CODE /////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.L))
        {
            reputation += 100;
            gold += 100;
            updateRessources();
        }
        ////////////////////////////////////////////////////////////////////////////////
    }

    public void StopBuilding()
    {
        isBuilding = false;
        houseToBuild = null;
        buildings.SetActive(false);
        spawnsBuildings.SetActive(false);
    }

    public void updateRessources()
    {
        textGold.text = gold.ToString();
        textReputation.text = reputation.ToString();
    }
}

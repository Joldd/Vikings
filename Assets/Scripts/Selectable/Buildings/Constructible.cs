using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Constructible : MonoBehaviour
{
    private bool firstPlayerBuilder;
    private bool firstEnemyBuilder;
    private bool needFirstPlayer;
    private bool needFirstEnemy;

    public bool enemyCapturing;
    public bool playerCapturing;

    [SerializeField] private Builder PF_builder;
    private Builder playerBuilder;
    private Builder enemyBuilder;

    [SerializeField] GameObject before;
    [SerializeField] GameObject after;
    private bool isConstructible;
    private bool isEmpty = true;

    [SerializeField] private GameObject btnBuildings;
    [SerializeField] private GameObject btnCanBuild;
    [SerializeField] private Image imageToBuild;
    [SerializeField] private List<HouseToBuild> L_HousesToBuild = new List<HouseToBuild>();
    private int currentImg = 0;

    public bool houseDestroy;

    [SerializeField] GameObject enemySpawner;

    [SerializeField] TextMeshProUGUI nameTMP;
    [SerializeField] TextMeshProUGUI costTMP;

    private GameManager gameManager;

    private Player owner;

    private List<Troop> L_troops = new List<Troop>();
    private List<Mark> L_Marks = new List<Mark>();


    private void Start()
    {
        btnBuildings.SetActive(false);
        btnCanBuild.SetActive(false);
        gameManager = GameManager.Instance;
    }

    public void ChangeOwnership(Player owner)
    {
        before.SetActive(false);
        after.SetActive(true);
        this.owner = owner;
        L_HousesToBuild = owner.GameSetup.Buildings;
        if (playerBuilder) Destroy(playerBuilder.gameObject);
        if (enemyBuilder) Destroy(enemyBuilder.gameObject);
        if (gameManager.CheckIsVicars(owner))
        {
            isConstructible = true;
            btnCanBuild.SetActive(true);

            //Manage Troops on constructible
            foreach (Troop troop in L_troops)
            {
                float x = transform.position.x - troop.transform.position.x;
                float z = transform.position.z - troop.transform.position.z;
                Vector3 newPos = troop.transform.position;
                if (x >= 0)
                {
                    newPos.x -= (2 - x);
                }
                else
                {
                    newPos.x += (2 - x);
                }
                if (z >= 0)
                {
                    newPos.z -= (2 - z);
                }
                else
                {
                    newPos.z += (2 - z);
                }

                troop.transform.position = newPos;
                if (troop.state == State.WAITING) troop.lastPositionMove = newPos;
            }

            //Manage Marks on constructible
            for (int i = L_Marks.Count - 1; i >= 0; i--)
            {
                Mark mark = L_Marks[i];
                if (mark == null)
                {
                    L_Marks.Remove(mark);
                    continue;
                }
                float x = transform.position.x - mark.transform.position.x;
                float z = transform.position.z - mark.transform.position.z;

                Vector3 newPos = mark.transform.position;
                if (x >= 0)
                {
                    newPos.x -= (1 - x);
                }
                else
                {
                    newPos.x += (1 - x);
                }
                if (z >= 0)
                {
                    newPos.z -= (1 - z);
                }
                else
                {
                    newPos.z += (1 - z);
                }

                mark.myWayPoints.myTroop.UpdateMarkPos(mark, newPos);
            }
        }
        else
        {
            if (playerBuilder) Destroy(playerBuilder.gameObject);
            if (enemyBuilder) Destroy(enemyBuilder.gameObject);
            GameObject enemyBuild = Instantiate(enemySpawner);
            House enemyHouse = enemyBuild.GetComponent<House>();
            enemyBuild.transform.position = transform.position;
            enemyHouse.constructible = this;
            enemyHouse.owner = this.owner;
            isEmpty = false;
        }
    }

    private void GoBackUnConstructible()
    {
        owner = null;
        before.SetActive(true);
        after.SetActive(false);
        firstPlayerBuilder = false;
        firstEnemyBuilder = false;
        isConstructible = false;
        btnCanBuild.SetActive(false);
        btnBuildings.SetActive(false);
        houseDestroy = false;
        isEmpty = true;
    }

    public void CanBeBuild()
    {
        if (isConstructible)
        {
            btnBuildings.SetActive(true);
            UpdateHouseToBuild();
            isConstructible = false;
            btnCanBuild.SetActive(false);
        }
    }

    private void UpdateHouseToBuild()
    {
        imageToBuild.sprite = L_HousesToBuild[currentImg].sprite;
        costTMP.text = L_HousesToBuild[currentImg].PB_House.GetComponent<EntityHouse>().priceReputation + " renommée";
        nameTMP.text = L_HousesToBuild[currentImg].PB_House.GetComponent<House>().name;
    }

    public void Build()
    {
        GameObject houseInstanced = Instantiate(L_HousesToBuild[currentImg].PB_House, transform);
        houseInstanced.transform.position = transform.position;
        House house = houseInstanced.GetComponent<House>();
        house.constructible = this;
        house.owner = this.owner;
        btnBuildings.SetActive(false);
        after.SetActive(false);
        isEmpty = false;
        gameManager.reputation -= L_HousesToBuild[currentImg].PB_House.GetComponent<EntityHouse>().priceReputation;
        gameManager.UpdateRessources();
    }

    public void ChangeRight()
    {
        currentImg++;
        if (currentImg >= L_HousesToBuild.Count)
        {
            currentImg = 0;
        }
        UpdateHouseToBuild();
    }

    public void ChangeLeft()
    {
        currentImg--;
        if (currentImg < 0)
        {
            currentImg = L_HousesToBuild.Count - 1;
        }
        UpdateHouseToBuild();
    }

    private void OnTriggerStay(Collider other)
    {
        ///////////// PAUSE /////////////////
        if (gameManager.isPause) return;

        if (other.TryGetComponent(out Troop troop))
        {
            owner = troop.owner;
            // If Vicars 
            if(gameManager.CheckIsVicars(troop.owner))
            {
                playerCapturing = true;
                if (!firstPlayerBuilder)
                {
                    needFirstPlayer = true;
                }
            }
            // If Vikings 
            else
            {
                enemyCapturing = true;
                if (!firstEnemyBuilder)
                {
                    needFirstEnemy = true;
                }
            }
            
            troop.areaToCapture = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check Troop
        if (other.TryGetComponent(out Troop troop))
        {
            if (gameManager.CheckIsVicars(troop.owner))
            {
                L_troops.Add(troop);
            }
        }
        //Check Marks
        if (other.TryGetComponent(out Mark mark))
        {
            L_Marks.Add(mark);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Troop troop))
        {
            if (gameManager.CheckIsVicars(troop.owner))
            {
                playerCapturing = false;
                L_troops.Remove(troop);
            }
            else
            {
                enemyCapturing = false;
            }
        }
        //Check Marks
        if (other.TryGetComponent(out Mark mark))
        {
            L_Marks.Remove(mark);
        }
    }

    private void Update()
    {
        if (houseDestroy)
        {
            GoBackUnConstructible();
        }

        if (!isEmpty) return;

        if (needFirstPlayer)
        {
            playerBuilder = Instantiate(PF_builder, gameManager.GetVicarBase().GetSpawnPosition(), Quaternion.identity);
            playerBuilder.owner = gameManager.VicarPlayer.Player;
            // playerBuilder.transform.position = basePlayer.transform.position;
            playerBuilder.constructible = this;
            firstPlayerBuilder = true;
            needFirstPlayer = false;
        }
        if (needFirstEnemy)
        {
            enemyBuilder = Instantiate(PF_builder, gameManager.GetVikingBase().GetSpawnPosition(), Quaternion.identity);
            enemyBuilder.owner = gameManager.VikingPlayer.Player;
            enemyBuilder.constructible = this;
            firstEnemyBuilder = true;
            needFirstEnemy = false;
        }

        if (playerCapturing && enemyCapturing)
        {
            if (playerBuilder) playerBuilder.isRunning = false;
            if (enemyBuilder) enemyBuilder.isRunning = false;
        }
        else if (playerCapturing && !enemyCapturing)
        {
            if (playerBuilder) playerBuilder.isRunning = true;
            if (enemyBuilder) enemyBuilder.isRunning = false;
        }
        else if (!playerCapturing && enemyCapturing)
        {
            if (enemyBuilder) enemyBuilder.isRunning = true;
            if (playerBuilder) playerBuilder.isRunning = false;
        }
        else if(!playerCapturing && !enemyCapturing)
        {
            if (playerBuilder) playerBuilder.isRunning = false;
            if (enemyBuilder) enemyBuilder.isRunning = false;
        }
    }
}

[System.Serializable]
public struct HouseToBuild
{
    public GameObject PB_House;
    public Sprite sprite;
}

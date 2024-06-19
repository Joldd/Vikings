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
    private bool isEmpty = true;

    [SerializeField] private GameObject btnBuildings;
    [SerializeField] private GameObject btnCanBuild;
    [SerializeField] private Image imageToBuild;
    [SerializeField] private List<HouseToBuild> L_HousesToBuild = new List<HouseToBuild>();
    private int currentImg = 0;

    [SerializeField] private Image imageCapture;
    private Color baseColor = new Color(1, 1, 1, 0.2f);

    public bool houseDestroy;

    [SerializeField] GameObject enemySpawner;

    [SerializeField] TextMeshProUGUI nameTMP;
    [SerializeField] TextMeshProUGUI costTMP;

    private GameManager gameManager;

    private Player owner;

    private void Start()
    {
        btnBuildings.SetActive(false);
        btnCanBuild.SetActive(false);
        gameManager = GameManager.Instance;

        imageCapture.color = baseColor;
    }

    public void DestroyBuilding()
    {
        after.SetActive(true);
        btnBuildings.SetActive(true);
        UpdateHouseToBuild();
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
            btnBuildings.SetActive(true);
            UpdateHouseToBuild();
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
        imageCapture.gameObject.SetActive(false);
    }

    private void GoBackUnConstructible()
    {
        owner = null;
        before.SetActive(true);
        after.SetActive(false);
        firstPlayerBuilder = false;
        firstEnemyBuilder = false;
        btnCanBuild.SetActive(false);
        btnBuildings.SetActive(false);
        houseDestroy = false;
        isEmpty = true;
        imageCapture.gameObject.SetActive(true);
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
        //Check Buikder
        if (other.TryGetComponent(out Builder builder))
        {
            builder._navMeshAgent.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Troop troop))
        {
            if (gameManager.CheckIsVicars(troop.owner))
            {
                playerCapturing = false;
            }
            else
            {
                enemyCapturing = false;
            }
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
            if (playerBuilder && playerBuilder.isRunning)
            {
                playerBuilder.Stop();
                imageCapture.color = baseColor;
            }
            if (enemyBuilder && enemyBuilder.isRunning)
            {
                enemyBuilder.Stop();
                imageCapture.color = baseColor;
            }
        }
        else if (playerCapturing && !enemyCapturing)
        {
            if (playerBuilder && !playerBuilder.isRunning)
            {
                playerBuilder.Go();
                imageCapture.color = new Color(1, 0.92f, 0.016f, 0.2f);
            }
            if (enemyBuilder && enemyBuilder.isRunning) enemyBuilder.Stop();
        }
        else if (!playerCapturing && enemyCapturing)
        {
            if (playerBuilder && playerBuilder.isRunning) playerBuilder.Stop();
            if (enemyBuilder && !enemyBuilder.isRunning)
            {
                imageCapture.color = new Color(1, 0, 0, 0.2f);
                enemyBuilder.Go();
            }
        }
        else if(!playerCapturing && !enemyCapturing)
        {
            if (playerBuilder && playerBuilder.isRunning)
            {
                playerBuilder.Stop();
                imageCapture.color = baseColor;
            }
            if (enemyBuilder && enemyBuilder.isRunning)
            {
                enemyBuilder.Stop();
                imageCapture.color = baseColor;
            }
        }
    }
}

[System.Serializable]
public struct HouseToBuild
{
    public GameObject PB_House;
    public Sprite sprite;
}

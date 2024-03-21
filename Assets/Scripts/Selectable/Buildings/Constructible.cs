using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Constructible : MonoBehaviour
{

    private GameObject basePlayer;
    private GameObject baseEnemy;

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

    [SerializeField] private GameObject HUD;
    [SerializeField] private Image imageToBuild;
    [SerializeField] private List<HouseToBuild> L_HousesToBuild = new List<HouseToBuild>();
    private int currentImg = 0;

    public bool houseDestroy;

    [SerializeField] GameObject enemySpawner;

    [SerializeField] TextMeshProUGUI nameTMP;
    [SerializeField] TextMeshProUGUI costTMP;

    private void Start()
    {
        basePlayer = GameObject.Find("BasePlayer");
        baseEnemy = GameObject.Find("BaseEnemy");
        HUD.SetActive(false);
    }

    public void BecomeConstructible()
    {
        before.SetActive(false);
        after.SetActive(true);
        if (playerBuilder) Destroy(playerBuilder.gameObject);
        if (enemyBuilder) Destroy(enemyBuilder.gameObject);
        isConstructible = true;
    }

    public void BecomeEnemy()
    {
        before.SetActive(false);
        after.SetActive(false);
        if (playerBuilder) Destroy(playerBuilder.gameObject);
        if (enemyBuilder) Destroy(enemyBuilder.gameObject);
        GameObject enemyBuild = Instantiate(enemySpawner);
        enemyBuild.transform.position = transform.position;
        enemyBuild.GetComponent<House>().constructible = this;
        isEmpty = false;
    }

    private void GoBackUnConstructible()
    {
        before.SetActive(true);
        after.SetActive(false);
        firstPlayerBuilder = false;
        firstEnemyBuilder = false;
        isConstructible = false;
        HUD.SetActive(false);
        houseDestroy = false;
        isEmpty = true;
    }

    private void OnMouseDown()
    {
        if (isConstructible)
        {
            HUD.SetActive(true);
            UpdateHouseToBuild();
            isConstructible = false;
        }
    }

    private void UpdateHouseToBuild()
    {
        imageToBuild.sprite = L_HousesToBuild[currentImg].sprite;
        costTMP.text = L_HousesToBuild[currentImg].house.priceReputation + " renommée";
        nameTMP.text = L_HousesToBuild[currentImg].house.name;
    }

    public void Build()
    {
        House house = Instantiate(L_HousesToBuild[currentImg].house, transform);
        house.transform.position = transform.position;
        house.constructible = this;
        HUD.SetActive(false);
        after.SetActive(false);
        isEmpty = false;
        GameManager.Instance.reputation -= house.priceReputation;
        GameManager.Instance.updateRessources();
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
        if (other.tag == "Player")
        {
            playerCapturing = true;
        }
        if (other.tag == "Enemy")
        {
            enemyCapturing = true;
        }

        if (other.tag == "Player" && !firstPlayerBuilder)
        {
            needFirstPlayer = true;
            other.GetComponent<Viking>().areaToCapture = this;
        }
        if (other.tag == "Enemy" && !firstEnemyBuilder)
        {
            needFirstEnemy = true;
            other.GetComponent<Viking>().areaToCapture = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCapturing = false;
        }
        if (other.tag == "Enemy")
        {
            enemyCapturing = false;
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
            playerBuilder = Instantiate(PF_builder);
            playerBuilder.isPlayer = true;
            playerBuilder.transform.position = basePlayer.transform.position;
            playerBuilder.constructible = this;
            firstPlayerBuilder = true;
            needFirstPlayer = false;
        }
        if (needFirstEnemy)
        {
            enemyBuilder = Instantiate(PF_builder);
            enemyBuilder.isPlayer = false;
            enemyBuilder.transform.position = baseEnemy.transform.position;
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
    public House house;
    public Sprite sprite;
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Constructible : MonoBehaviour
{

    private GameObject basePlayer;
    private GameObject baseEnemy;
    private bool firstPlayerBuilder;
    private bool firstEnemyBuilder;
    public bool enemyCapturing;
    public bool playerCapturing;
    [SerializeField] private Builder PF_builder;
    private Builder playerBuilder;
    private Builder enemyBuilder;

    [SerializeField] GameObject before;
    [SerializeField] GameObject after;
    private bool isConstructible;

    [SerializeField] private GameObject HUD;
    [SerializeField] private Image imageToBuild;
    [SerializeField] private List<HouseToBuild> L_HousesToBuild = new List<HouseToBuild>();
    private int currentImg = 0;

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
        firstEnemyBuilder = false;
        firstEnemyBuilder = false;
        isConstructible = true;
    }

    private void OnMouseDown()
    {
        if (isConstructible)
        {
            HUD.SetActive(true);
            imageToBuild.sprite = L_HousesToBuild[currentImg].sprite;
            isConstructible = false;
        }
    }

    public void Build()
    {
        House house = Instantiate(L_HousesToBuild[currentImg].house, transform);
        house.transform.position = transform.position;
        HUD.SetActive(false);
        after.SetActive(false);
    }

    public void ChangeRight()
    {
        currentImg++;
        if (currentImg >= L_HousesToBuild.Count)
        {
            currentImg = 0;
        }
        imageToBuild.sprite = L_HousesToBuild[currentImg].sprite;
    }

    public void ChangeLeft()
    {
        currentImg--;
        if (currentImg < 0)
        {
            currentImg = L_HousesToBuild.Count - 1;
        }
        imageToBuild.sprite = L_HousesToBuild[currentImg].sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!firstPlayerBuilder && other.tag == "Player")
        {
            firstPlayerBuilder = true;
            playerBuilder = Instantiate(PF_builder);
            playerBuilder.transform.position = basePlayer.transform.position;
            playerBuilder.constructible = this;
        }
        if (!firstEnemyBuilder && other.tag == "Enemy")
        {
            firstEnemyBuilder = true;
            enemyBuilder = Instantiate(PF_builder);
            enemyBuilder.transform.position = baseEnemy.transform.position;
            enemyBuilder.constructible = this;
        }
        if (other.tag == "Player")
        {
            playerCapturing = true;
            other.GetComponent<Viking>().areaToCapture = this;
        }
        if (other.tag == "Enemy")
        {
            enemyCapturing = true;
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

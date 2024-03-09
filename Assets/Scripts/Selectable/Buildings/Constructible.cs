using UnityEngine;

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


    private void Start()
    {
        basePlayer = GameObject.Find("BasePlayer");
        baseEnemy = GameObject.Find("BaseEnemy");
    }

    public void BecomeConstructible()
    {
        before.SetActive(false);
        after.SetActive(true);
        Destroy(playerBuilder.gameObject);
        Destroy(enemyBuilder.gameObject);
        firstEnemyBuilder = false;
        firstEnemyBuilder = false;
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

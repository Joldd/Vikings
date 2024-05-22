using UnityEngine;

public class SpawnerEnnemy : MonoBehaviour
{
    [SerializeField] float timerMax;
    float timer;
    [SerializeField] Troop troop;
    [SerializeField] EntityUnit ennemyToSpawn;
    [SerializeField] float rotation;

    private GameManager gameManager;

    private void Start()
    {
        timer = timerMax;

        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            EntityUnit ennemySpawned = Instantiate(ennemyToSpawn);
            Troop myTroop = Instantiate(troop);
            myTroop.owner = gameManager.VikingPlayer.Player;
            myTroop.transform.position = transform.position;
            myTroop.tag = "Enemy";  
            myTroop.type = ennemySpawned.type;
            myTroop.AddUnit(ennemySpawned);
            timer = timerMax;
        }
    }
}

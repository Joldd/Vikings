using UnityEngine;

public class SpawnerEnnemy : MonoBehaviour
{
    [SerializeField] float timerMax;
    float timer;
    [SerializeField] Troop troop;
    [SerializeField] EntityUnit ennemyToSpawn;
    [SerializeField] float rotation;

    private void Start()
    {
        timer = timerMax;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            EntityUnit ennemySpawned = Instantiate(ennemyToSpawn);
            Troop myTroop = Instantiate(troop);
            myTroop.owner = GameManager.Instance.VikingPlayer.Player;
            myTroop.transform.position = transform.position;
            myTroop.tag = "Enemy";  
            myTroop.type = ennemySpawned.type;
            myTroop.AddUnit(ennemySpawned);
            ennemySpawned.transform.rotation = transform.rotation;
            myTroop.transform.Rotate(ennemySpawned.transform.up, rotation);
            timer = timerMax;
        }
    }
}

using UnityEngine;

public class SpawnerEnnemy : MonoBehaviour
{
    [SerializeField] float timerMax;
    float timer;
    [SerializeField] GameObject ennemyToSpawn;
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
            GameObject ennemySpawned = Instantiate(ennemyToSpawn);
            ennemySpawned.transform.position = transform.position;
            ennemySpawned.transform.rotation = transform.rotation;
            ennemySpawned.transform.Rotate(ennemySpawned.transform.up, rotation);
            timer = timerMax;
        }
    }
}

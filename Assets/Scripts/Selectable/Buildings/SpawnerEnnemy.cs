using UnityEngine;

public class SpawnerEnnemy : MonoBehaviour
{
    [SerializeField] float timerMax;
    float timer;
    [SerializeField] GameObject ennemyToSpawn;

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
            timer = timerMax;
        }
    }
}

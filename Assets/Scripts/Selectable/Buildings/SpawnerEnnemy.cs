using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerEnnemy : MonoBehaviour
{
    [SerializeField] private float timerMaxBaseUnit, timerMaxTroop, timerMaxGuardTroop;

    [SerializeField] private float maxTroopPercentageSpawn, baseTroopPercentageSpawn, guardTroopPercentageSpawn;
    
    private float timerTroop;

    private UnityAction spawnAction;

    [SerializeField] AITroop troop;
    [SerializeField] EntityUnit baseUnitToSpawn;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        ComputeRandomSpawnType();
    }

    private void Update()
    {
        timerTroop -= Time.deltaTime;
        if(timerTroop < 0)
        {
            spawnAction.Invoke();
            ComputeRandomSpawnType();
        }
    }

    private void ComputeRandomSpawnType()
    {
        List<float> percentageList = new List<float>();
        
        percentageList.AddRange(new []{maxTroopPercentageSpawn, baseTroopPercentageSpawn, guardTroopPercentageSpawn});

        switch (SelectPercentage(percentageList))
        {
            case 0 : 
                // Max Troop 
                spawnAction = () => SpawnTroop();
                timerTroop = timerMaxTroop;
                break;
            case 1 :
                // Base Troop 
                spawnAction = () => SpawnBaseUnit();
                timerTroop = timerMaxBaseUnit;
                break;
            case 2 : 
                // Guard Troop 
                spawnAction = () => SpawnGuardTroop();
                timerTroop = timerMaxGuardTroop;
                break;
        }
    }
    
    private int SelectPercentage(List<float> votePercentages)
    {
        float random = Random.Range(0f,100f);
        
        double cumulative = 0;
        int index = 0;
        foreach (var percentage in votePercentages)
        {
            cumulative += percentage;
            if (random < cumulative)
            {
                return index;
            }
            index++;
        }

        return 0;
    }

    private AITroop SpawnBaseUnit()
    {
        EntityUnit spawnedUnit = Instantiate(baseUnitToSpawn);
        AITroop myTroop = Instantiate(troop);
        myTroop.owner = gameManager.VikingPlayer.Player;
        myTroop.transform.position = transform.position;
        myTroop.tag = "Enemy";  
        myTroop.type = spawnedUnit.type;
        myTroop.AddUnit(spawnedUnit);

        myTroop.SetAIState(AIEnemyState.RushBase);
        return myTroop;
    }

    private void SpawnTroop()
    {
        AITroop myTroop = Instantiate(troop);
        myTroop.owner = gameManager.VikingPlayer.Player;
        myTroop.transform.position = transform.position;
        myTroop.tag = "Enemy";

        for (int i = 0; i < 3; i++)
        {
            EntityUnit spawnedUnit = Instantiate(baseUnitToSpawn);
            myTroop.AddUnit(spawnedUnit);
        }

        myTroop.type = baseUnitToSpawn.type;
        myTroop.SetAIState(AIEnemyState.RushBase);
    }

    private void SpawnGuardTroop()
    {
        AITroop myTroop = SpawnBaseUnit();
        myTroop.SetAIState(AIEnemyState.Guard);
       // myTroop.SetAsGuard();  // Assuming you have a method to set the troop as a guard
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasureGoal", menuName = "ScriptableObjects/TreasureGoal", order = 1)]
public class SOTreasureGoal : SOGameGoal
{
    [SerializeField] private int treasureGoal;
    private int actualNbTreasure;
    
    public override void SetupCheckGoalDone(Player playerRef)
    {
        //Reset values
        base.SetupCheckGoalDone(playerRef);
        actualNbTreasure = 0;
        
        gameManager.AddListenerTreasureGoal(CheckNbTreasureObtained);
    }
    
    private void CheckNbTreasureObtained(Player playerRef)
    {
        if (playerRef == this.playerRef)
        {
            actualNbTreasure++;
            if (actualNbTreasure == treasureGoal)
            {
                //TODO Call Game Manager to end the game
                gameManager.PlayerWinGame(playerRef);
            }
        }
    }
}

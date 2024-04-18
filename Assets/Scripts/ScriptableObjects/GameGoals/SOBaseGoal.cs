using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseGoal", menuName = "ScriptableObjects/BaseGoal", order = 1)]
public class SOBaseGoal : SOGameGoal
{
    [SerializeField] private int nbBaseDestroyed;
    private int actualNbBaseDestroyed;

    public override void SetupCheckGoalDone(Player playerRef)
    {
        //Reset values
        base.SetupCheckGoalDone(playerRef);
        actualNbBaseDestroyed = 0;
        
        gameManager.AddListenerBaseGoal(CheckNbBaseDestroyed);
    }

    private void CheckNbBaseDestroyed(Player playerRef)
    {
        if (playerRef == this.playerRef)
        {
            actualNbBaseDestroyed++;
            if (actualNbBaseDestroyed == nbBaseDestroyed)
            {
                //TODO Call Game Manager to end the game
                gameManager.PlayerWinGame(playerRef);
            }
        }
    }
    
}

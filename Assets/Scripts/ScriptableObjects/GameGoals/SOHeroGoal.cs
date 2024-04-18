using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroGoal", menuName = "ScriptableObjects/HeroGoal", order = 1)]
public class SOHeroGoal : SOGameGoal
{
    [SerializeField] private int heroKilledGoal;
    private int actualNbHeroKilled;
    
    public override void SetupCheckGoalDone(Player playerRef)
    {
        base.SetupCheckGoalDone(playerRef);
        actualNbHeroKilled = 0;
        gameManager.AddListenerHeroGoal(CheckNbHeroKilled);
    }
    
    private void CheckNbHeroKilled(Player playerRef)
    {
        if (playerRef == this.playerRef)
        {
            actualNbHeroKilled++;
            if (actualNbHeroKilled == heroKilledGoal)
            {
                //TODO Call Game Manager to end the game
                gameManager.PlayerWinGame(playerRef);
            }
        }
    }
}

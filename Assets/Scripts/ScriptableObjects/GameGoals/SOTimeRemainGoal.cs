using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeRemainGoal", menuName = "ScriptableObjects/TimeRemainGoal", order = 1)]
public class SOTimeRemainGoal : SOGameGoal
{
    [SerializeField] private float timerGoal;
    
    public override void SetupCheckGoalDone(Player playerRef)
    {
        base.SetupCheckGoalDone(playerRef);
        
        gameManager.AddListenerTimeRemainGoal(CheckTimeGoal);
    }


    private void CheckTimeGoal(float timer)
    {
        if (timer >= timerGoal)
        {
            gameManager.PlayerWinGame(playerRef);
        }
    }
}

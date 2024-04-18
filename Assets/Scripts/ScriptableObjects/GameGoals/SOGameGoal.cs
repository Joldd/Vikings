using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOGameGoal : ScriptableObject
{
    protected GameManager gameManager;
    protected Player playerRef;
    public virtual void SetupCheckGoalDone(Player playerRef)
    {
        gameManager = GameManager.Instance;
        this.playerRef = playerRef;
    }
}

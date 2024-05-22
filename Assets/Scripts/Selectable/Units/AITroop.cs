using UnityEngine;

public enum AIEnemyState
{
    Guard,
    RushBase,
    Patrol
}

public class AITroop : Troop
{
    public AIEnemyState aIState;

    public override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
        
        //////////////////   ENEMY   ////////////////////////////////////
        if (state == State.ENEMY)
        {
            switch (aIState)
            {
                case AIEnemyState.RushBase:
                    transform.position = Vector3.MoveTowards(transform.position, gameManager.basePlayer.transform.position, speed * Time.deltaTime);
                    transform.LookAt(gameManager.basePlayer.transform.position);
                    PlayAnimation("Run");
                    break;
                case AIEnemyState.Guard :
                    break;
            }
        }
    }

    public void SetAIState(AIEnemyState state)
    {
        aIState = state;
    }
}
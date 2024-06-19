
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
                    NavMeshAgent.isStopped = false;
                    if (gameManager.GetVicarBase() != null)
                    {
                        NavMeshAgent.SetDestination(gameManager.GetVicarBase().GetSpawnPosition());
                        PlayAnimation("Run");
                    }
                    break;
                case AIEnemyState.Guard :
                    PlayAnimation("Idle");
                    break;
            }
        }
    }

    public void SetAIState(AIEnemyState state)
    {
        aIState = state;
    }
}
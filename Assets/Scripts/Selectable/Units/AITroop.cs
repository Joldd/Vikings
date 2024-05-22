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

        //Enemy Detection Sphere
        if (!checkEnemy && type != Type.Messenger)
        {
            Vector3 boxCenter = transform.position + transform.forward * 3;
            Vector3 boxSize = Vector3.one * flankRange;
            Quaternion boxOrientation = transform.rotation;

            //Enemy Detection Forward Box
            RaycastHit[] hitsBox = Physics.BoxCastAll(boxCenter, boxSize, transform.forward, boxOrientation, 3, layerMaskTroopTarget);

            foreach (var hit in hitsBox)
            {
                Troop enemyTroop = null;
                if (hit.transform.gameObject.TryGetComponent(out enemyTroop))
                {
                    if (enemyTroop.owner != owner && enemyTroop.type != Type.Messenger)
                    {
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        state = State.RUNATTACK;
                        PlayAnimation("Run");
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);
                        // Check flank 
                        TargetEnemyFlank(Vector3.Angle(enemyTroop.transform.forward, transform.forward));
                        enemyTroop.GetTargeted(this);
                        checkEnemy = true;
                        break;
                    }
                }
            }
            Vector3[] cubePoint = CubePoints(boxCenter, boxSize, boxOrientation);
            DrawCubePoints(cubePoint);

            RaycastHit[] hitsSphere = Physics.SphereCastAll(transform.position, aoeRange / 2, transform.up, 10000, layerMaskTroopTarget);

            foreach (var hit in hitsSphere)
            {
                if (hit.transform.gameObject.TryGetComponent(out Troop enemyTroop))
                {
                    if (enemyTroop.owner != owner && enemyTroop.type != Type.Messenger)
                    {
                        enemyTroop = hit.transform.gameObject.GetComponent<Troop>();
                        target = enemyTroop.GetNearestUnitFromTroop(transform.position);

                    }
                }

                if (hit.transform.gameObject.TryGetComponent(out EntityHouse enemyBuilding))
                {
                    if (enemyBuilding.House.owner != owner)
                    {
                        target = enemyBuilding;
                    }
                }

                if (target != null)
                {
                    state = State.RUNATTACK;
                    PlayAnimation("Run");
                    GiveTarget();
                    checkEnemy = true;
                }
            }
        }
    }

    public void SetAIState(AIEnemyState state)
    {
        aIState = state;
    }
}
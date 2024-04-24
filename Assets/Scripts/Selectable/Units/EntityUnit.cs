using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum Type
{
    Paladin,
    Guard,
    Mutant,
    Messenger,
    Hero
}

public class EntityUnit : Entity
{
    public State state = State.SLEEPING;

    public Entity target;
    public float timerAttackMax;

    public float speed;
    public float range;
    public int aoeRange;
    public int flankRange;
    public int damage = 1;
    public int maxTroop;

    private float damageMultiplier = 1;
    public GameObject body;

    [SerializeField] bool enemyStop;

    [SerializeField] int goldToWin;

    public Type type;

    public Troop myTroop;

    public Outline outline;

    private GameManager gameManager;

    [Header("Floor Spd Modifier")]
    [SerializeField] private LayerMask floorMask;
    private float spdMultiplier = 1;
    private TerrainFloor actualTerrainFloor;

    public float Speed
    {
        get => speed * spdMultiplier;
    }

    public override void Start()
    {
        base.Start();

        healthBar.UpdateValue();

        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();

        outline = GetComponent<Outline>();
        gameManager = GameManager.Instance;
        if (outline) outline.OutlineMode = Outline.Mode.Nothing; 
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, floorMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            if (hit.transform.TryGetComponent(out TerrainFloor floor))
            {
                if (actualTerrainFloor == null || actualTerrainFloor != floor)
                {
                    actualTerrainFloor = floor;
                    spdMultiplier = floor.GetSpdMultiplierFromType(type);
                    if (myTroop != null)
                    {
                        myTroop.UpdateSpeedTroop(speed * spdMultiplier);
                    }
                }
            }
        }
    }
    
    public void Attack()
    {
        animator.speed = 1f / timerAttackMax;
        animator.Play("Attack");
    }

    public float GetDamage()
    {
        return damage * damageMultiplier;
    }

    public void ResetBonusDmg()
    {
        damageMultiplier = 1;
    }

    public void AddBonusDmgFlank(float newMulti)
    {
        damageMultiplier = newMulti;
    }

    public override void Die()
    {
        base.Die();

        if(!gameManager.CheckIsVicars(myTroop.owner))
        {
            gameManager.gold += goldToWin;
            gameManager.UpdateRessources();
        }

        if (myTroop) myTroop.RemoveUnit(this);
    }
}

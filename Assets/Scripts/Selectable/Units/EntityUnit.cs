using System;
using UnityEngine;

public enum Type
{
    Paladin,
    Guard,
    Mutant,
    Messenger,
    Hero,
    Archer
}

[Serializable]
public class EntityUnitBuff
{
    public int additionalEnvironmentDamage = 0;
    public int additionalEnvironmentArmor = 0;
    public int additionalUpgradeDamage = 0;
    public int additionalUpgradeArmor = 0;
    public float spdMultiplier = 1;
    
    public float damageMultiplier = 1;
    public float attackSpeedMutliplier = 1;
    public float rangeMutliplier = 1;
}

public class EntityUnit : Entity
{
    public State state = State.SLEEPING;

    public Sprite imageUnit;
    public Entity target;
    public float timerAttackMax;
    
    public float speed;
    public float range;
    public int aoeRange;
    public int flankRange;
    public int damage = 1;
    public int armor = 0;
    public int maxTroop;
    private EntityUnitBuff unitBuffs;

    public GameObject body;

    [SerializeField] bool enemyStop;

    [SerializeField] int goldToWin;

    [SerializeField] 
    public Type type;

    public Troop myTroop;

    public Outline outline;

    protected GameManager gameManager;

    [Header("Floor System")]
    [SerializeField] private LayerMask floorMask;
    private TerrainFloor actualTerrainFloor;
    
    public int AdditionalEnvironmentDamage { get => unitBuffs.additionalEnvironmentDamage; set => unitBuffs.additionalEnvironmentDamage = value; }
    public int AdditionalEnvironmentArmor { get => unitBuffs.additionalEnvironmentArmor; set => unitBuffs.additionalEnvironmentArmor = value; }
    public float Speed { get => speed * unitBuffs.spdMultiplier; }
    public float AttackSpeed { get => timerAttackMax * unitBuffs.attackSpeedMutliplier; }
    public float Range { get => range * unitBuffs.rangeMutliplier; }
    public float AttackDamage { get => (damage + unitBuffs.additionalEnvironmentDamage) * unitBuffs.damageMultiplier; }
    public float Armor { get => (armor + unitBuffs.additionalEnvironmentArmor); }
    
    public (bool,bool) IsAttackModified { get => (unitBuffs.additionalEnvironmentDamage > 0 || unitBuffs.additionalUpgradeDamage > 0 || unitBuffs.damageMultiplier > 1, AttackDamage > damage); }
    public (bool,bool) IsAttackSpeedModified { get => (unitBuffs.attackSpeedMutliplier > 1, AttackSpeed > timerAttackMax); }
    public (bool,bool) IsSpeedModified { get => (unitBuffs.attackSpeedMutliplier > 1, Speed > speed); }
    public (bool,bool) IsArmorModified { get => (unitBuffs.additionalEnvironmentDamage > 0 || unitBuffs.additionalUpgradeArmor > 0, Armor > armor); }
    public (bool,bool) IsRangeModified { get => (unitBuffs.rangeMutliplier > 1, Range > range); }


    public override void Start()
    {
        base.Start();

        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();

        outline = GetComponent<Outline>();
        gameManager = GameManager.Instance;
        unitBuffs = new EntityUnitBuff();
        if (outline) outline.OutlineMode = Outline.Mode.Nothing; 
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, floorMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
            if (hit.transform.TryGetComponent(out TerrainFloor floor))
            {
                if (actualTerrainFloor == null || actualTerrainFloor != floor)
                {
                    actualTerrainFloor = floor;
                    if (floor.DoesSpdModifierContainType(type))
                    {
                        unitBuffs.spdMultiplier = floor.GetSpdMultiplierFromType(type);
                        if (myTroop != null)
                        {
                            myTroop.UpdateSpeedTroop(Speed);
                        }
                    }

                    if (floor.DoesBuffModifierContainType(type))
                    {
                        AdditionalEnvironmentDamage = floor.GetBonusStatsFromType(type).attack;
                        AdditionalEnvironmentArmor = floor.GetBonusStatsFromType(type).armor;
                    }
                    else
                    {
                        AdditionalEnvironmentDamage = 0;
                        AdditionalEnvironmentArmor = 0;
                    }
                }
            }
        }
    }
    
    public void Attack()
    {
        if (!animator.isActiveAndEnabled) return;
        animator.speed = 1f / timerAttackMax;
        animator.Play("Attack");
    }

    public float GetDamage()
    {
        return damage * unitBuffs.damageMultiplier;
    }

    // 35 is a specific value 
    public float GetMitigatedDamage(float damage)
    {
        return damage / (1f + Armor / 35f);
    }


    public void ResetBonusDmg()
    {
        unitBuffs.damageMultiplier = 1;
    }

    public void AddBonusDmgFlank(float newMulti)
    {
        unitBuffs.damageMultiplier = newMulti;
    }

    public override void Die()
    {
        base.Die();

        if(!gameManager.CheckIsVicars(myTroop.owner))
        {
            gameManager.gold += goldToWin;
            gameManager.UpdateRessources();
        }

        if (myTroop) myTroop.DisableUnit(this);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);    

        if (myTroop) myTroop.UpdateHealthBarTroop();
    }
}

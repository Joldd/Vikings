using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

public enum AttackType
{
    SoloUnit,
    MultiUnit,
}

public enum AttackEffect
{
    DirectDamage,
    DoTDamage
}
public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;
    [SerializeField] string soundAttack;
    
    [Header("Attack Effects")]
    [SerializeField] AttackType attackType;
    [SerializeField] float aoeRange = 0;
    [SerializeField] AttackEffect attackEffect;
    [SerializeField] GameObject vfxDamageEffectPrefab;
    ParticleSystem vfxDamageEffect;
    [SerializeField] LayerMask layerMaskTroopTarget; 
    
    [SerializeField] Arrow arrow;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }
    public void Deal()
    {
        if (!unit.target) return;

        audioManager.Play(soundAttack);

        if (unit.target.PV > 0)
        {
            int damage = 0;
            if (unit.target.TryGetComponent(out EntityUnit entityUnit))
            {
                damage = (int) entityUnit.GetMitigatedDamage(unit.GetDamage());
            }
            else
            {
                damage = (int)unit.GetDamage();
            }

            if (vfxDamageEffectPrefab != null && vfxDamageEffect == null)
            {
                vfxDamageEffect = Instantiate(vfxDamageEffectPrefab, unit.target.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            }
            
            if (vfxDamageEffect != null)
            {
                vfxDamageEffect.transform.position = unit.target.transform.position;
                vfxDamageEffect.Play();
            }
            
            unit.target.TakeDamage(damage);

            // Check AOE
            if (attackType == AttackType.MultiUnit)
            {
                RaycastHit[] hitsSphere =
                    Physics.SphereCastAll(unit.target.transform.position, aoeRange / 2, transform.up, 10000, layerMaskTroopTarget);
                foreach (var hit in hitsSphere)
                {
                    if (hit.transform.gameObject.TryGetComponent(out EntityUnit entityUnitAoe))
                    {
                        if (entityUnitAoe.myTroop.owner != unit.myTroop.owner)
                        {
                            entityUnitAoe.TakeDamage(entityUnitAoe.GetMitigatedDamage(unit.GetDamage()));
                        }
                    }
                    if (hit.transform.gameObject.TryGetComponent(out EntityHouse entityHouseAoe))
                    {
                        if (entityHouseAoe.House.owner != unit.myTroop.owner)
                        {
                            entityHouseAoe.TakeDamage(damage);
                        }
                    }
                }

                if (unit.TryGetComponent<Hero>(out Hero hero))
                {
                    hero.UpdatePVHero();
                }

                //Enemy aggro
                if (unit.target.TryGetComponent<EntityUnit>(out EntityUnit enemy))
                {
                    enemy.myTroop.GetTargeted(unit.myTroop);
                }
            }

        }
    }

    public void ShootProjectile()
    {
        Arrow instancedArrow = Instantiate(arrow);
        instancedArrow.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + 1.5f, unit.transform.position.z);
        instancedArrow.unit = unit;
        instancedArrow.unitHelper = this;
    }
}

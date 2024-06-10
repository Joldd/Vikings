using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;
    [SerializeField] AudioSource soundAttack;
    [SerializeField] Arrow arrow;
    public void Deal()
    {
        if (!unit.target) return;

        if (soundAttack) soundAttack.Play();

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
            unit.target.TakeDamage(damage);

            if(unit.TryGetComponent<Hero>(out Hero hero))
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

    public void ShootProjectile()
    {
        Arrow instancedArrow = Instantiate(arrow);
        instancedArrow.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + 1.5f, unit.transform.position.z);
        instancedArrow.unit = unit;
        instancedArrow.unitHelper = this;
    }
}

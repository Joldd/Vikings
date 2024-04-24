using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;

    public void Deal()
    {
        if (!unit.target) return;
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
            unit.target.PV -= damage;
            unit.target.healthBar.UpdateValue();

            if(unit.TryGetComponent<Hero>(out Hero hero))
            {
                hero.UpdatePVHero();
            }
        }
    }
}

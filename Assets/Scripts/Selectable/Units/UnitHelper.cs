using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;
    [SerializeField] AudioSource soundAttack;

    public void Deal()
    {
        if (!unit.target) return;

        soundAttack.Play();

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
        }
    }
}

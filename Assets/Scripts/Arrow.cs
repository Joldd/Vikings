using UnityEngine;

public class Arrow : MonoBehaviour
{
    public EntityUnit unit;
    private int speed = 22;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, unit.target.transform.position, speed * Time.deltaTime);
        transform.LookAt(unit.target.transform);

        if (Vector3.Distance(transform.position, unit.target.transform.position) <= unit.target.size)
        {
            if (unit.target.PV > 0)
            {
                int damage = 0;
                if (unit.target.TryGetComponent(out EntityUnit entityUnit))
                {
                    damage = (int)entityUnit.GetMitigatedDamage(unit.GetDamage());
                }
                else
                {
                    damage = (int)unit.GetDamage();
                }
                unit.target.TakeDamage(damage);

                if (unit.TryGetComponent<Hero>(out Hero hero))
                {
                    hero.UpdatePVHero();
                }
            }
            Destroy(gameObject);
        }
    }
}

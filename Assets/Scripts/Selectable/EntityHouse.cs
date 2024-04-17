
using UnityEngine;

public class EntityHouse : Entity
{
    private House house;

    public override void Start()
    {
        base.Start();

        house = GetComponent<House>();

        if (!isBuilt) animator.Play("Build");
    }

    public override void Die()
    {
        base.Die();

        if (house.constructible) house.constructible.houseDestroy = true;

        if (house.isBase && tag == "Player") UIManager.Instance.Defeat();

        if (house.isBase && tag == "Enemy") UIManager.Instance.Victory();
    }

    private void Update()
    {
        /////////////////////////  BuildingHouse ///////////////////////
        if (!isBuilt)
        {
            timeBuild -= Time.deltaTime;
            healthBar.slider.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }
        if (timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
        }
    }
}


using UnityEngine;

public class EntityHouse : Entity
{
    private House house;
    private GameManager gameManager;

    public House House { get => house; }

    public override void Start()
    {
        base.Start();

        house = GetComponent<House>();
        gameManager = GameManager.Instance;
        if (!isBuilt) animator.Play("Build");
    }

    public override void Die()
    {
        base.Die();

        if (house.constructible) house.constructible.houseDestroy = true;

        if (house.isBase)
        {
            Debug.LogError("Entity House is Destroyed");
            gameManager.onBaseIsDestroyed.Invoke( gameManager.CheckIsVicars(house.owner) ? gameManager.VikingPlayer.Player : gameManager.VicarPlayer.Player);
        }
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

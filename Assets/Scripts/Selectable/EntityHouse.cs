using UnityEngine;

public class EntityHouse : Entity
{
    public Color healthBarColor;
    public HealthBarHouse healthBarHousePrefab;
    [HideInInspector] public HealthBarHouse healthBarHouse;

    [SerializeField] private House house;
    private GameManager gameManager;

    public bool isRepairing;
    public bool canRepair = true;
    private LayerMask layerTroop;
    private float timerCheckEnemiesMax = 1f;
    private float timerCheckEnemies;
    private float timerRepairMax = 0.5f;
    private float timeRepair;

    public House House { get => house; }

    private GameObject flameEffect;
    private bool isFlaming;

    public override void Start()
    {
        base.Start();

        layerTroop = LayerMask.GetMask("Troop");

        gameManager = GameManager.Instance;

        if (!isBuilt) animator.Play("Build");
        flameEffect = transform.Find("Flame").gameObject;
        flameEffect.SetActive(false);

        timerCheckEnemies = timerCheckEnemiesMax;
        timeRepair = timerRepairMax;
        
        //Health Bar
        healthBarHouse = Instantiate(healthBarHousePrefab, transform.position, Quaternion.identity);
        healthBarHouse.StartBar(gameObject, healthBarColor);
        healthBarHouse.UpdateValue();
    }

    public void Sell()
    {
        gameManager.reputation += Mathf.RoundToInt((float)priceReputation / 2f);
        gameManager.UpdateRessources();
        house.UnSelect();
        Die();
    }

    public override void Die()
    {
        base.Die();

        if (house.constructible) house.constructible.houseDestroy = true;

        if (house.isBase)
        {
            gameManager.onBaseIsDestroyed.Invoke( gameManager.CheckIsVicars(house.owner) ? gameManager.VikingPlayer.Player : gameManager.VicarPlayer.Player);
        }
        
        Destroy(healthBarHouse.gameObject);
        Destroy(gameObject);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBarHouse.UpdateValue();
    }

    private void Update()
    {
        /////////////////////////  BuildingHouse ///////////////////////
        if (!isBuilt)
        {
            timeBuild -= Time.deltaTime;
            healthBarHouse.slider.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }
        if (timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
        }

        //FLAME IF DAMAGED
        if (!isFlaming && (float)PV < (float)maxPV / 2)
        {
            flameEffect.SetActive(true);
            isFlaming = true;
        }
        if (isFlaming && (float)PV > (float)maxPV / 2)
        {
            flameEffect.SetActive(false);
            isFlaming = false;
        }

        //CHECK IF ENEMIES AROUND
        timerCheckEnemies -= Time.deltaTime;
        if (timerCheckEnemies <= 0)
        {
            RaycastHit[] hitsSphere = Physics.SphereCastAll(transform.position, 10f, transform.up, 10000, layerTroop);
            canRepair = true;
            foreach (RaycastHit hit in hitsSphere)
            {
                if (hit.transform.gameObject.TryGetComponent(out Troop enemyTroop))
                {
                    if (enemyTroop.owner != house.owner && enemyTroop.type != Type.Messenger)
                    {
                        canRepair = false;
                        isRepairing = false;
                        timeRepair = timerRepairMax;
                        break;
                    }
                }
            }
            timerCheckEnemies = timerCheckEnemiesMax;
        }

        //REPAIR
        if (isRepairing)
        {
            timeRepair -= Time.deltaTime;
            if (timeRepair <= 0)
            {
                PV++;
                gameManager.reputation--;
                timeRepair = timerRepairMax;
                healthBarHouse.UpdateValue();
                gameManager.UpdateRessources();
            }
            if (gameManager.reputation <= 0 || PV >= maxPV) isRepairing = false;
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hero : EntityUnit
{
    [SerializeField] private GameObject mainMenu;
    private GameObject heroMenu;
    private Slider heroLife;
    private GameObject panelRespawn;
    public TextMeshProUGUI textTimer;
    public bool isDie;
    public float timerRespawnMax;
    public float timerRespawn;
    [SerializeField] ParticleSystem respawnParticle;
    [SerializeField] AudioSource respawnSound;

    public override void Start()
    {
        base.Start();

        heroMenu = mainMenu.transform.Find("Hero").gameObject;
        heroMenu.GetComponent<HeroButton>().myHero = this;
        heroLife = heroMenu.transform.Find("Slider").gameObject.GetComponent<Slider>();
        panelRespawn = heroMenu.transform.Find("Respawn").gameObject;
        textTimer = panelRespawn.transform.Find("Timer").gameObject.GetComponent<TextMeshProUGUI>();

        panelRespawn.SetActive(false);

        timerRespawn = timerRespawnMax;

        gameManager = GameManager.Instance;

        respawnParticle.Stop();

        UpdatePVHero();
    }

    public void UpdatePVHero()
    {
        heroLife.value = (float)PV / (float)maxPV;
    }

    [ContextMenu("Respawn")]
    public void Respawn()
    {
        isDie = false;
        panelRespawn.gameObject.SetActive(false);
        myTroop.healthBar.gameObject.SetActive(true);
        myTroop.gameObject.SetActive(true);
        PV = maxPV;
        myTroop.UpdateHealthBarTroop();
        UpdatePVHero();
        respawnParticle.Play();
        respawnSound.Play();
        myTroop.ResetNavMesh();
        myTroop.isWaypoints = false;
        myTroop.checkEnemy = false;
    }

    [ContextMenu("Die")]
    public override void Die()
    {
        panelRespawn.SetActive(true);
        myTroop.healthBar.gameObject.SetActive(false);
        isDie = true;
        myTroop.transform.position = gameManager.CheckIsVicars(myTroop.owner) ? gameManager.GetVicarBase().GetSpawnPosition() : gameManager.GetVikingBase().GetSpawnPosition();
        myTroop.gameObject.SetActive(false);
        myTroop.ResetTroop();
    }
}


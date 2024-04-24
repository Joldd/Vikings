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
    int maxPV;
    public bool isDie;
    public float timerRespawnMax;
    public float timerRespawn;
    GameManager gameManager;

    public override void Start()
    {
        base.Start();

        heroMenu = mainMenu.transform.Find("Hero").gameObject;
        heroLife = heroMenu.transform.Find("Slider").gameObject.GetComponent<Slider>();
        panelRespawn = heroMenu.transform.Find("Respawn").gameObject;
        textTimer = panelRespawn.transform.Find("Timer").gameObject.GetComponent<TextMeshProUGUI>();

        panelRespawn.SetActive(false);

        maxPV = PV;

        timerRespawn = timerRespawnMax;

        gameManager = GameManager.Instance;
    }

    public void UpdatePVHero()
    {
        heroLife.value = (float)PV / (float)maxPV;
    }

    public void Respawn()
    {
        isDie = false;
        panelRespawn.gameObject.SetActive(false);
        myTroop.gameObject.SetActive(true);
        PV = maxPV;
        healthBar.UpdateValue();
        UpdatePVHero();
    }

    public override void Die()
    {
        panelRespawn.SetActive(true);
        isDie = true;
        myTroop.transform.position = gameManager.basePlayer.transform.Find("Spawn").position;
        myTroop.gameObject.SetActive(false);
    }

}


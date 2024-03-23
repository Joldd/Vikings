using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Units;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    
    Animator animator;

    Selectable currentUnit;
    Slider currentSliderUnit = null;
    bool isBuilding = false;

    public Constructible constructible;

    private float timer = 0;

    public bool isBase;

    private List<Viking> L_Vikings = new List<Viking>();
    private List<Troop> L_Troop = new List<Troop>();

    [SerializeField] private Troop troopGO;

    public override void Start()
    {
        base.Start();

        //Health Bar
        healthBar = Instantiate(healthBar, transform.position, Quaternion.identity);
        healthBar.StartBar(gameObject);

        canvas = transform.Find("CanvasBuilding").gameObject;

        animator = GetComponent<Animator>();

        if (!isBuilt) animator.Play("Build");

        if (L_Units.Count == L_Buttons.Count)
        {
            for (int i = 0; i < L_Buttons.Count; i++)
            {
                Slider sliderUnit = L_Buttons[i].transform.Find("Slider").GetComponent<Slider>();
                sliderUnit.gameObject.SetActive(false);
                int n = i;
                L_Buttons[i].onClick.AddListener(() =>
                {
                    if (!isBuilding && GameManager.Instance.gold >= L_Units[n].GetComponent<Selectable>().priceGold)
                    {
                        if (L_Units[n])
                        {
                            currentUnit = L_Units[n].GetComponent<Selectable>();
                            sliderUnit.gameObject.SetActive(true);
                            currentSliderUnit = sliderUnit;
                            isBuilding = true;
                            timer = currentUnit.timeBuildMax;
                            GameManager.Instance.gold -= currentUnit.priceGold;
                            GameManager.Instance.updateRessources();
                        }
                    }
                });
            }
        }
        else
        {
            Debug.LogWarning("The Building " + name + " miss units or buttons");
        }
    }

    public override void Die()
    {
        base.Die();

        if (constructible) constructible.houseDestroy = true;

        if (isBase && tag == "Player") UIManager.Instance.Defeat();

        if (isBase && tag == "Enemy") UIManager.Instance.Victory();
    }

    private void createUnit(Selectable unit, Transform spawn)
    {
        Selectable s = Instantiate(unit);
        s.transform.position = spawn.transform.position;
        bool createTroop = false;
        bool isTroop = false;
        if (s.TryGetComponent<Viking>(out Viking v))
        {
            foreach(Troop t in L_Troop)
            {
                if (t.type == v.type)
                {
                    isTroop = true;
                    t.AddUnit(v);
                }
            }
            if (isTroop) return;
            foreach(Viking vik in L_Vikings)
            {
                if (vik.type == v.type)
                {
                    Troop troop = Instantiate(troopGO);
                    L_Troop.Add(troop);
                    troop.type = v.type;
                    troop.transform.position = spawn.transform.position;
                    troop.AddUnit(vik);
                    troop.AddUnit(v);
                    createTroop = true;
                    L_Vikings.Remove(vik);
                    break;
                }
            }
            if (!createTroop)
            {
                L_Vikings.Add(v);
            }
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
        if ( timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
        }

        /////////////////////////  BuildingViking ///////////////////////
        if (isBuilding)
        {
            timer -= Time.deltaTime;

            currentSliderUnit.value = (currentUnit.timeBuildMax - timer) / currentUnit.timeBuildMax;

            if (currentSliderUnit.value >= 1)
            {
                createUnit(currentUnit, spawn);
                currentSliderUnit.gameObject.SetActive(false);
                isBuilding = false;
                currentSliderUnit = null;
                currentUnit = null;
            }
        }
    }
}

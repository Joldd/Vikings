using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    [SerializeField] public List<EntityUnit> L_EntityUnits;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;

    EntityUnit currentUnit;
    Slider currentSliderUnit = null;
    bool isBuilding = false;

    public Constructible constructible;

    private float timer = 0;

    public bool isBase;

    private List<Troop> L_Troop = new List<Troop>();

    [SerializeField] private Troop troopGO;

    public override void Start()
    {
        base.Start();

        canvas.SetActive(false);

        canvas = transform.Find("CanvasBuilding").gameObject;

        if (L_EntityUnits.Count == L_Buttons.Count)
        {
            for (int i = 0; i < L_Buttons.Count; i++)
            {
                Slider sliderUnit = L_Buttons[i].transform.Find("Slider").GetComponent<Slider>();
                sliderUnit.gameObject.SetActive(false);
                int n = i;
                L_Buttons[i].onClick.AddListener(() =>
                {
                    if (!isBuilding && GameManager.Instance.gold >= L_EntityUnits[n].priceGold
                        && (!GetTroop(L_EntityUnits[n].GetComponent<EntityUnit>())
                            || GetTroop(L_EntityUnits[n].GetComponent<EntityUnit>()).L_Units.Count < GetTroop(L_EntityUnits[n].GetComponent<EntityUnit>()).maxTroop))
                    {
                        if (L_EntityUnits[n])
                        {
                            currentUnit = L_EntityUnits[n];
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

    private Troop GetTroop(EntityUnit v)
    {
        foreach (Troop t in L_Troop)
        {
            if (t.type == v.type)
            {
                return t;
            }
        }
        return null;
    }

    private void createUnit(EntityUnit unit, Transform spawn)
    {
        EntityUnit u = Instantiate(unit);
        u.transform.position = spawn.transform.position;
        bool isTroop = false;
        if (u.TryGetComponent<EntityUnit>(out EntityUnit v))
        {
            //IF ALREADY TROOP
            foreach(Troop t in L_Troop)
            {
                if (t.type == v.type)
                {
                    isTroop = true;
                    t.AddUnit(v);
                }
            }
            if (isTroop) return;

            //ELSE CREATE TROOP
            Troop troop = Instantiate(troopGO);
            L_Troop.Add(troop);
            troop.type = v.type;
            troop.transform.position = spawn.transform.position;
            troop.AddUnit(v);
            troop.myHouse = this;
        }
    }

    public void DetachTroop(Troop troop)
    {
        L_Troop.Remove(troop);
        troop.myHouse = null;
    }

    private void Update()
    {
        /////////////////////////  BuildingUnit && Troop ///////////////////////
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

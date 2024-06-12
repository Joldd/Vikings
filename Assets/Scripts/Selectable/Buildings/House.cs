using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    [SerializeField] Transform spawn;

    EntityUnit currentUnit;
    public Slider currentSliderUnit = null;
    bool isBuilding = false;

    public Constructible constructible;

    private float timer = 0;

    public bool isBase;

    private List<Troop> L_Troop = new List<Troop>();

    [SerializeField] private Troop troopGO;

    private GameManager gameManager;
    private UIManager uiManager;

    public ButtonUnit[] myButtonsToCreate;
    public List<GameObject> myButtonsToDisplay = new List<GameObject>();

    public override void Start()
    {
        base.Start();

        gameManager = GameManager.Instance;
        uiManager = UIManager.Instance;

        uiManager.CreateButtonsBuilding(this);
    }

    public void StartBuild(EntityUnit unit)
    {
        if (!isBuilding && gameManager.gold >= unit.priceGold
                        && (!GetTroop(unit.GetComponent<EntityUnit>())
                            || GetTroop(unit.GetComponent<EntityUnit>()).L_Units.Count < GetTroop(unit.GetComponent<EntityUnit>()).maxTroop))
        {
            if (unit)
            {
                currentUnit = unit;
                isBuilding = true;
                timer = currentUnit.timeBuildMax;
                gameManager.gold -= currentUnit.priceGold;
                gameManager.UpdateRessources();
            }
        }
    }

    public override void Select()
    {
        base.Select();

        uiManager.DisplayUIBuilding(this, true);
    }

    public override void UnSelect()
    {
        base.UnSelect();

        uiManager.DisplayUIBuilding(this, false);
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
        EntityUnit u = Instantiate(unit, spawn.transform.position,Quaternion.identity);
        bool isTroop = false;
        if (u.TryGetComponent<EntityUnit>(out EntityUnit v))
        {
            //IF ALREADY TROOP
            foreach(Troop t in L_Troop)
            {
                if (t.type == v.type)
                {
                    isTroop = true;
                    StartCoroutine(t.AddUnit(v));
                }
            }
            if (isTroop) return;

            //ELSE CREATE TROOP
            Troop troop = Instantiate(troopGO, spawn.transform.position, Quaternion.identity);
            L_Troop.Add(troop);
            troop.owner = owner;
            troop.type = v.type;
            StartCoroutine(troop.AddUnit(v));
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

    public Vector3 GetSpawnPosition()
    {
        return spawn.position;
    }
}

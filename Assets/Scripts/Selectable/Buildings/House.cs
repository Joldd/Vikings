using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class House : Selectable
{
    [SerializeField] Transform spawn;

    EntityUnit currentUnit;
    public Slider currentSliderUnit = null;
    public BuildingUnit currentBuildingUnit = null;
    public bool isBuilding = false;
    public List<BuildingUnit> L_WaitListUnits = new List<BuildingUnit>();

    public Constructible constructible;

    private float timer = 0;

    public bool isBase;

    private List<Troop> L_Troop = new List<Troop>();

    [SerializeField] private Troop troopGO;

    private UIManager uiManager;
    private AudioManager audioManager;

    public ButtonUnit[] myButtonsToCreate;
    public List<GameObject> myButtonsToDisplay = new List<GameObject>();

    public override void Start()
    {
        base.Start();

        uiManager = UIManager.Instance;
        audioManager = AudioManager.Instance;

        uiManager.CreateButtonsBuilding(this);
    }

    public void StartBuild(EntityUnit unit)
    {
        if (!isBuilding && (!GetTroop(unit.GetComponent<EntityUnit>()) || GetTroop(unit.GetComponent<EntityUnit>()).L_Units.Count < GetTroop(unit.GetComponent<EntityUnit>()).maxTroop))
        {
            if (unit)
            {
                currentUnit = unit;
                isBuilding = true;
                timer = currentUnit.timeBuildMax;
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

    public void StopBuildUnit(BuildingUnit buildingUnit)
    {
        if (buildingUnit == currentBuildingUnit)
        {
            isBuilding = false;
            currentSliderUnit = null;
            currentUnit = null;
            Destroy(currentBuildingUnit.gameObject);
            currentBuildingUnit = null;
            gameManager.gold += buildingUnit.myEntityUnit.priceGold;
            gameManager.UpdateRessources();
        }
        else
        {
            Destroy(buildingUnit.gameObject);
            L_WaitListUnits.Remove(buildingUnit);
            gameManager.gold += buildingUnit.myEntityUnit.priceGold;
            gameManager.UpdateRessources();
        }
    }

    private void createUnit(EntityUnit unit, Transform spawn)
    {
        EntityUnit u = Instantiate(unit, spawn.transform.position,Quaternion.identity);
        if (u.soundBuilding != null) audioManager.Play(u.soundBuilding);
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
                isBuilding = false;
                currentSliderUnit = null;
                currentUnit = null;
                Destroy(currentBuildingUnit.gameObject);
                currentBuildingUnit = null;
            }
        }

        ///Build WaitList
        if (L_WaitListUnits.Count > 0 && !isBuilding)
        {
            if (!GetTroop(L_WaitListUnits[0].myEntityUnit.GetComponent<EntityUnit>()) ||
                GetTroop(L_WaitListUnits[0].myEntityUnit.GetComponent<EntityUnit>()).L_Units.Count < GetTroop(L_WaitListUnits[0].myEntityUnit.GetComponent<EntityUnit>()).maxTroop)
            {
                currentSliderUnit = L_WaitListUnits[0].mySlider;
                currentBuildingUnit = L_WaitListUnits[0];
                StartBuild(L_WaitListUnits[0].myEntityUnit);
                L_WaitListUnits.RemoveAt(0);
            }
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return spawn.position;
    }
}

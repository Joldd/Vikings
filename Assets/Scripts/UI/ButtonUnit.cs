using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonUnit
{
    [SerializeField] EntityUnit unit;
    [SerializeField] Sprite sprite;
    [SerializeField] string nameUnit;
    [SerializeField] GameObject BtnPF;
    [SerializeField] GameObject buildingUnitPF;
    private GameManager gameManager;
    private int maxWaiting = 4;

    public GameObject SetupButton(Transform parentButton, Transform parentBuildingUnit, House house)
    {
        gameManager = GameManager.Instance;

        GameObject btnUnitGO = GameObject.Instantiate(BtnPF, parentButton);
        Button btn = btnUnitGO.GetComponent<Button>();

        Image img = btnUnitGO.GetComponent<Image>();
        img.sprite = sprite;

        HoverTitle hoverTitle = btnUnitGO.AddComponent<HoverTitle>();
        hoverTitle.unit = unit;
        hoverTitle.hover_pos = Hover_Pos.UPRIGHT;

        TextMeshProUGUI title = btnUnitGO.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        title.text = nameUnit;

        btn.onClick.AddListener(() =>
        {
            if (unit.priceGold > gameManager.gold) return;
            if (house.L_WaitListUnits.Count >= maxWaiting) return;

            gameManager.gold -= unit.priceGold;
            gameManager.UpdateRessources();

            GameObject buildingUnitGO = GameObject.Instantiate(buildingUnitPF, parentBuildingUnit);
            BuildingUnit buildingUnit = buildingUnitGO.GetComponent<BuildingUnit>();
            buildingUnit.myHouse = house;
            buildingUnit.myImage.sprite = sprite;
            buildingUnit.myEntityUnit = unit;

            if (!house.isBuilding) 
            {
                house.currentSliderUnit = buildingUnit.mySlider;
                house.currentBuildingUnit = buildingUnit;
                house.StartBuild(unit);
            }
            else
            {
                house.L_WaitListUnits.Add(buildingUnit);
            }
        });

        return btnUnitGO;
    }
}

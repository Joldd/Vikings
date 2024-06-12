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


    public GameObject SetupButton(Transform parent, House house)
    {
        GameObject btnUnitGO = GameObject.Instantiate(BtnPF, parent);
        Button btn = btnUnitGO.GetComponent<Button>();

        GameObject sliderGO = btnUnitGO.transform.Find("Slider").gameObject;
        sliderGO.SetActive(false);

        Slider slider = sliderGO.GetComponent<Slider>();

        Image img = btnUnitGO.GetComponent<Image>();
        img.sprite = sprite;

        HoverTitle hoverTitle = btnUnitGO.AddComponent<HoverTitle>();
        hoverTitle.unit = unit;

        TextMeshProUGUI title = btnUnitGO.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        title.text = nameUnit;

        btn.onClick.AddListener(() =>
        {
            sliderGO.SetActive(true);
            house.currentSliderUnit = slider;
            house.StartBuild(unit);
        });

        return btnUnitGO;
    }
}

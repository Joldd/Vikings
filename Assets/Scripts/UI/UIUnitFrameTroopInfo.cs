using UnityEngine;
using UnityEngine.UI;

public class UIUnitFrameTroopInfo : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private Image fillImage;

    [SerializeField] private Color killedColor;
    [SerializeField] private Color aliveColor;
    

    //TODO Faire une ecoute d'un event quand l'unit prend des dégats 


    public void SetupUnitData(EntityUnit unit)
    {
        image.sprite = unit.imageUnit;
        UpdateSliderValue((float) unit.PV/unit.maxPV);
        unit.AddListenerOnDamageTaken(UpdateSliderValue);
    }
    
    private void UpdateSliderValue(float value)
    {
        slider.value = value;
        image.color = value > 0 ? aliveColor : killedColor;
        fillImage.enabled = value > 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitFrameTroopInfo : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    
    //TODO Faire une ecoute d'un event quand l'unit prend des dégats 


    public void SetupUnitData(EntityUnit unit)
    {
        image.sprite = unit.imageUnit;
        slider.value = (float) unit.PV/unit.maxPV;
        unit.AddListenerOnDamageTaken(UpdateSliderValue);
    }
    
    private void UpdateSliderValue(float value)
    {
        slider.value = value;
    }
}

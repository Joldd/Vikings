using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Slider slider;
    float maxPV;

    private void Start()
    {
        slider = transform.Find("HUDCharacter").Find("HealthBar").GetComponent<Slider>();
        maxPV = GetComponent<Selectable>().PV;
    }

    public void updateHealthBar()
    {
        slider.value = GetComponent<Selectable>().PV / maxPV;
    }
}

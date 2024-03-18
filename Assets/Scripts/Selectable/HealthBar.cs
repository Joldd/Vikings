using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private GameObject canvas;
    public Slider slider;
    float maxPV;

    private void Start()
    {
        canvas = transform.Find("HUD").gameObject;
        canvas.SetActive(true);
        slider = transform.Find("HUD").Find("HealthBar").GetComponent<Slider>();
        maxPV = GetComponent<Selectable>().PV;
    }

    public void updateHealthBar()
    {
        slider.value = GetComponent<Selectable>().PV / maxPV;
    }

    private void Update()
    {
        canvas.transform.LookAt(Camera.main.transform);
    }
}

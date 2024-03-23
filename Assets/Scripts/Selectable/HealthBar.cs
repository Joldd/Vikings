using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthSprite;
    Unit unit;
    RectTransform rectTransform;
    [Range(0, 5)] public float healthBarUpOffset;
    RectTransform canvasRectTransform;
    public Slider slider;
    float maxPV;

    void Update()
    {
        Vector3 worldTargetPosition = unit.transform.position + Vector3.up * healthBarUpOffset;
        rectTransform.position = Camera.main.WorldToScreenPoint(worldTargetPosition);
    }

    public void StartBar(GameObject target)
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<HealthBarCanvas>().rectTransform;
        transform.SetParent(canvasRectTransform);
        unit = target.GetComponent<Unit>();
        maxPV = unit.PV;
    }

    public void UpdateValue()
    {
        slider.value = (float)unit.PV / (float)maxPV;
    }
}
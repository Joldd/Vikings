using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthSprite;
    Selectable selectable;
    RectTransform rectTransform;
    [Range(0, 5)] public float healthBarUpOffset;
    RectTransform canvasRectTransform;
    public Slider slider;
    float maxPV;

    void Update()
    {
        Vector3 worldTargetPosition = selectable.transform.position + Vector3.up * healthBarUpOffset;
        rectTransform.position = Camera.main.WorldToScreenPoint(worldTargetPosition);
    }

    public void StartBar(GameObject target)
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<HealthBarCanvas>().rectTransform;
        transform.SetParent(canvasRectTransform);
        selectable = target.GetComponent<Selectable>();
        maxPV = selectable.PV;
    }

    public void UpdateValue()
    {
        slider.value = selectable.PV / maxPV;
    }
}
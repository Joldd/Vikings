using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthSprite;
    Entity unit;
    RectTransform rectTransform;
    [Range(0, 5)] public float healthBarUpOffset;
    RectTransform canvasRectTransform;
    public Slider slider;
    float maxPV;
    [SerializeField] GameObject blocToHide;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        Vector3 worldTargetPosition = unit.transform.position + Vector3.up * healthBarUpOffset;
        rectTransform.position = Camera.main.WorldToScreenPoint(worldTargetPosition);

        if (unit && unit.tag == "Enemy" && unit.TryGetComponent<EntityUnit>(out EntityUnit e))
        {
            if (gameManager.fogWar.CheckVisibility(worldTargetPosition, 1))
            {
                blocToHide.SetActive(true);
            }
            else
            {
                blocToHide.SetActive(false);
            }
        }
    }

    public void StartBar(GameObject target)
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<HealthBarCanvas>().rectTransform;
        transform.SetParent(canvasRectTransform);
        unit = target.GetComponent<Entity>();
        maxPV = unit.PV;
    }

    public void UpdateValue()
    {
        slider.value = (float)unit.PV / (float)maxPV;
    }
}
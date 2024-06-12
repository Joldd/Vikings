using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthSprite;
    private Troop troopUnity;
    protected RectTransform rectTransform;
    [Range(0, 5)] public float healthBarUpOffset;
    protected RectTransform canvasRectTransform;
    public Slider slider;
    [SerializeField] protected GameObject blocToHide;
    [SerializeField] Image bonus;
    public Image fillImage;

    protected GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void Bonus(FlankValues flankValue)
    {
        switch (flankValue)
        {
            case FlankValues.BACK:
                bonus.gameObject.SetActive(true);
                bonus.color = Color.red;
                    break;
            case FlankValues.FRONT:
                bonus.gameObject.SetActive(false);
                break;
            case FlankValues.SIDES:
                bonus.gameObject.SetActive(true);
                bonus.color = Color.blue;
                break;
        }
    }

    void Update()
    {
        Vector3 worldTargetPosition = troopUnity.transform.position + Vector3.up * healthBarUpOffset;
        rectTransform.position = Camera.main.WorldToScreenPoint(worldTargetPosition);

        if (troopUnity && troopUnity.tag == "Enemy" && troopUnity.GetComponent<Troop>() != null)
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

    public virtual void StartBar(GameObject target, Color color)
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<HealthBarCanvas>().rectTransform;
        transform.SetParent(canvasRectTransform);
        troopUnity = target.GetComponent<Troop>();
        fillImage.color = color;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Hover_Pos
{
    DOWNRIGHT,
    DOWNLEFT,
    UPRIGHT,
    UPLEFT
}

public class HoverTitle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver = false;
    [SerializeField] string text;
    public Entity unit;

    private GameManager gameManager;
    private UIManager uIManager;

    public Hover_Pos hover_pos;

    private void Start()
    {
        gameManager = GameManager.Instance;
        uIManager = UIManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        TextMeshProUGUI textHover = uIManager.panelHover.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if (unit != null)
        {
            if (unit.priceGold > 0)
            {
                textHover.text = unit.priceGold.ToString();
                uIManager.panelHover.transform.Find("Gold").gameObject.SetActive(true);
                uIManager.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
                if (gameManager.gold < unit.priceGold)
                {
                    textHover.color = Color.red;
                }
                else
                {
                    textHover.color = Color.green;
                }
            }
            if (unit.priceReputation > 0)
            {
                textHover.text = unit.priceReputation.ToString();
                uIManager.panelHover.transform.Find("Reputation").gameObject.SetActive(true);
                uIManager.panelHover.transform.Find("Gold").gameObject.SetActive(false);
                if (gameManager.reputation < unit.priceReputation)
                {
                    textHover.color = Color.red;
                }
                else
                {
                    textHover.color = Color.green;
                }
            }
        }
        else
        {
            textHover.text = text;
            textHover.color = Color.black;
            uIManager.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
            uIManager.panelHover.transform.Find("Gold").gameObject.SetActive(false);
        }
        uIManager.panelHover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        uIManager.panelHover.SetActive(false);
        gameManager.objectHover = null;
    }

    private void Update()
    {
        if (isOver)
        {
            gameManager.objectHover = this;
            switch (hover_pos)
            {
                case Hover_Pos.DOWNRIGHT:
                        uIManager.panelHover.transform.position = new Vector3(Input.mousePosition.x + 100, Input.mousePosition.y - 50, Input.mousePosition.z);
                        break;
                case Hover_Pos.DOWNLEFT:
                    uIManager.panelHover.transform.position = new Vector3(Input.mousePosition.x - 50, Input.mousePosition.y - 50, Input.mousePosition.z);
                    break;
                case Hover_Pos.UPRIGHT:
                    uIManager.panelHover.transform.position = new Vector3(Input.mousePosition.x + 50, Input.mousePosition.y + 25, Input.mousePosition.z);
                    break;
                case Hover_Pos.UPLEFT:
                    uIManager.panelHover.transform.position = new Vector3(Input.mousePosition.x - 50, Input.mousePosition.y + 25, Input.mousePosition.z);
                    break;
            }
        }
    }
}

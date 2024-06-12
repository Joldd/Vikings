using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTitle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver = false;
    [SerializeField] string text;
    public Entity unit;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        TextMeshProUGUI textHover = gameManager.panelHover.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if (unit != null)
        {
            if (unit.priceGold > 0)
            {
                textHover.text = unit.priceGold.ToString();
                gameManager.panelHover.transform.Find("Gold").gameObject.SetActive(true);
                gameManager.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
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
                gameManager.panelHover.transform.Find("Reputation").gameObject.SetActive(true);
                gameManager.panelHover.transform.Find("Gold").gameObject.SetActive(false);
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
            gameManager.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
            gameManager.panelHover.transform.Find("Gold").gameObject.SetActive(false);
        }
        gameManager.panelHover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        gameManager.panelHover.SetActive(false);
        gameManager.objectHover = null;
    }

    private void Update()
    {
        if (isOver)
        {
            gameManager.objectHover = this;
            gameManager.panelHover.transform.position = new Vector3(Input.mousePosition.x + 100, Input.mousePosition.y - 50, Input.mousePosition.z);
        }
    }
}

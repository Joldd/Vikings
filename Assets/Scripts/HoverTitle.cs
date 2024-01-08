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
    [SerializeField] Selectable unit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        TextMeshProUGUI textHover = GameManager.Instance.panelHover.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if (unit != null)
        {
            if (unit.priceGold > 0)
            {
                textHover.text = unit.priceGold.ToString();
                GameManager.Instance.panelHover.transform.Find("Gold").gameObject.SetActive(true);
                GameManager.Instance.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
                if (GameManager.Instance.gold < unit.priceGold)
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
                GameManager.Instance.panelHover.transform.Find("Reputation").gameObject.SetActive(true);
                GameManager.Instance.panelHover.transform.Find("Gold").gameObject.SetActive(false);
                if (GameManager.Instance.reputation < unit.priceReputation)
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
            GameManager.Instance.panelHover.transform.Find("Reputation").gameObject.SetActive(false);
            GameManager.Instance.panelHover.transform.Find("Gold").gameObject.SetActive(false);
        }
        GameManager.Instance.panelHover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        GameManager.Instance.panelHover.SetActive(false);
        GameManager.Instance.objectHover = null;
    }

    private void Update()
    {
        if (isOver)
        {
            GameManager.Instance.objectHover = this;
            GameManager.Instance.panelHover.transform.position = new Vector3(Input.mousePosition.x + 100, Input.mousePosition.y - 50, Input.mousePosition.z);
        }
    }
}

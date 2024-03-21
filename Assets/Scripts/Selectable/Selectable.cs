using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject select;
    public GameObject canvas;

    public bool isSelect;

    public bool canBeSelected;

    public int PV;

    public float timeBuildMax;
    public bool isBuilt = false;
    
    public float timeBuild;

    public int priceReputation;
    public int priceGold;

    public HealthBar healthBar;

    private Outline outline;

    public virtual void Start()
    {
        canBeSelected = true;

        canvas.SetActive(false);

        timeBuild = timeBuildMax;

        select = transform.Find("Select").gameObject;
        select.SetActive(false);

        if (tag == "Enemy")
        {
            canBeSelected = false;
        }
        else
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.yellow;
            noOutLine();
        }
    }

    public void noOutLine()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.Nothing;
        }
    }

    private void goOutLine()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 2;
        }
    }

    public virtual void Die()
    {
        Destroy(healthBar.gameObject);
    }

    public virtual void Select()
    {
        GameManager.Instance.selectedUnit = this;
        select.SetActive(true);
        isSelect = true;
        canvas.SetActive(true);
    }

    public virtual void UnSelect()
    {
        select.SetActive(false);
        isSelect = false;
        canvas.SetActive(false);
    }

    public virtual void OnMouseDown()
    {
        if (canBeSelected)
        {
            Selectable selectedUnit = GameManager.Instance.selectedUnit;

            if (selectedUnit != null)
            {
                if (selectedUnit.TryGetComponent<Messenger>(out Messenger messenger))
                {
                    if (GameManager.Instance.isChoosingMessager)
                    {
                        messenger.vikingSelected = this.GetComponent<Viking>();
                        messenger.StopChooseViking();
                        return;
                    }
                }
                GameManager.Instance.selectedUnit.UnSelect();
            }
            GameManager.Instance.StopBuilding();
            Select();
        }
    }

    private void OnMouseOver()
    {
        goOutLine();
    }

    private void OnMouseExit()
    {
        noOutLine();
    }
}

using UnityEngine;

public class Selectable : MonoBehaviour
{
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

    private void hoverOutline()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 2;
        }
    }

    public void selectOutline()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 4;
        }
    }

    public virtual void Die()
    {
        Destroy(healthBar.gameObject);
    }

    public virtual void Select()
    {
        GameManager.Instance.selectedUnit = this;
        selectOutline();
        isSelect = true;
        canvas.SetActive(true);
    }

    public virtual void UnSelect()
    {
        noOutLine();
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
        if (!isSelect)
        {
            hoverOutline();
        }
    }

    private void OnMouseExit()
    {
        if (!isSelect)
        {
            noOutLine();
        }
    }
}

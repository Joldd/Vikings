using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject canvas;

    public bool isSelect;

    public bool canBeSelected;

    public Outline outline;

    public virtual void Start()
    {
        canBeSelected = true;

        if (tag == "Enemy")
        {
            canBeSelected = false;
        }

        outline = GetComponent<Outline>();
        if (outline) noOutLine();    
    }

    public virtual void noOutLine()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.Nothing;
        }
    }

    public virtual void hoverOutline()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 2;
        }
    }

    public virtual void selectOutline()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = 4;
        }
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
                if (selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
                {
                    if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                    {
                        if (GameManager.Instance.isChoosingMessager)
                        {
                            messenger.troopSelected = this.GetComponent<Troop>();
                            messenger.StopChooseTroop();
                            return;
                        }
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

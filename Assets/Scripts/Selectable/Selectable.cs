using Unity.VisualScripting;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject canvas;

    public bool isSelect;

    public bool canBeSelected;

    public Outline outline;

    public Player owner;

    private GameManager gameManager;

    public virtual void Start()
    {
        gameManager = GameManager.Instance;
        canBeSelected = true;

        if (!gameManager.CheckIsVicars(owner))
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
        gameManager.selectedUnit = this;
        selectOutline();
        isSelect = true;
        if (canvas) canvas.SetActive(true);
    }

    public virtual void UnSelect()
    {
        noOutLine();
        isSelect = false;
        if (canvas) canvas.SetActive(false);
    }

    public virtual void OnMouseDown()
    {
        if (canBeSelected)
        {
            Selectable selectedUnit = gameManager.selectedUnit;

            if (selectedUnit != null)
            {
                if (selectedUnit.TryGetComponent<Troop>(out Troop troopMsg))
                {
                    if (troopMsg.L_Units[0].TryGetComponent<Messenger>(out Messenger messenger))
                    {
                        if (messenger.troopChoosen && TryGetComponent<Troop>(out Troop troop))
                        {
                            if (troop.type == Type.Messenger) return;

                            messenger.troopSelected = troop;
                            messenger.StopChooseTroop();
                            gameManager.ChangeCursor(gameManager.cursorNormal);
                            return;
                        }
                    }
                }
                gameManager.selectedUnit.UnSelect();
            }
            gameManager.StopBuilding();
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

    public Vector3 RayTheFloor(int layerMask)
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}

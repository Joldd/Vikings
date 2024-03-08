using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject select;
    public GameObject canvas;

    public bool isSelect;

    public bool canBeSelected = false;

    public int PV;

    public float timeBuildMax;
    public bool isBuilt = false;
    
    public float timeBuild;

    public int priceReputation;
    public int priceGold;

    public HealthBar healthBar;

    public virtual void Start()
    {
        canvas.SetActive(false);

        timeBuild = timeBuildMax;

        select = transform.Find("Select").gameObject;

        healthBar = GetComponent<HealthBar>();
    }

    public virtual void Die()
    {
        Debug.Log(gameObject.name + " is dead");
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

    public virtual void Update()
    {
        if ( isBuilt && tag != "Enemy" )
        {
            canBeSelected = true;
        }
    }
}

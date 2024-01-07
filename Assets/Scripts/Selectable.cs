using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    public virtual void Start()
    {
        canvas.SetActive(false);

        timeBuild = timeBuildMax;

        select = transform.Find("Select").gameObject;
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
            if (GameManager.Instance.selectedUnit != null)
            {
                GameManager.Instance.selectedUnit.UnSelect();
            }
            GameManager.Instance.StopBuilding();
            Select();
        }
    }
}

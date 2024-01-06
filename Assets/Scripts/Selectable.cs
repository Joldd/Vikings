using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] GameObject select;
    [SerializeField] GameObject canvas;

    public bool isSelect;

    public bool canBeSelected;

    public int PV;

    public virtual void Start()
    {
        canvas.SetActive(false);
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

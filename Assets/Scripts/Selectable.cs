using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Selectable : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject select;

    public bool isSelect{ 
        get{ 
            return false ; 
        } 
        set { ; } 
    }

    public int PV
    {
        get
        {
            return 1;
        }
        set {; }
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
    }

    public void UnSelect()
    {
        select.SetActive(false);
        isSelect = false;
    }
}

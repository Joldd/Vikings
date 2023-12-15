using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] GameObject select;

    public void Select()
    {
        GameManager.Instance.selectedUnit = this;
        select.SetActive(true);
    }

    public void Unselect()
    {
        select.SetActive(false);
    }
}

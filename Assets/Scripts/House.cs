using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Unit
{
    [SerializeField] Viking viking;

    private void OnMouseDown()
    {
        if (GameManager.Instance.selectedUnit != null)
        {
            GameManager.Instance.selectedUnit.Unselect();
        }
        Select();
        GameManager.Instance.buttonsHouse.gameObject.SetActive(true);
        GameManager.Instance.ButtonsUnit.gameObject.SetActive(false);

    }
}

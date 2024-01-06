using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void buildMenu()
    {
        GameManager.Instance.buildings.SetActive(!GameManager.Instance.buildings.activeSelf);
    }

    public void build(House house)
    {
        if (GameManager.Instance.reputation >= house.priceReputation)
        {
            GameManager.Instance.spawnsBuildings.SetActive(true);
            GameManager.Instance.isBuilding = true;
            GameManager.Instance.houseToBuild = house;
        }
    }
}

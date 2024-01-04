using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnBuilding : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameManager.Instance.buildings.SetActive(false);
        House house = Instantiate(GameManager.Instance.houseToBuild);
        house.transform.position = new Vector3(transform.position.x, transform.position.y+0.7f, transform.position.z);
        GameManager.Instance.isBuilding = false;
        GameManager.Instance.houseToBuild = null;
        transform.parent.gameObject.SetActive(false);
    }
}

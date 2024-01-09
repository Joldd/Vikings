using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VikingHelper : MonoBehaviour
{
    [SerializeField] Viking viking;

    public void Deal()
    {
        viking.target.PV -= viking.damage;
        viking.target.healthBar.updateHealthBar();
    }

}

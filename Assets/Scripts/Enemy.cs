using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] float maxPV = 5f;
    public float PV;

    private void Start()
    {
        PV = maxPV;
    }

    public void updateHealthBar()
    {
        slider.value = PV / maxPV;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SpdMultiplierPerUnit
{
    [SerializeField] private Type unitType;
    [SerializeField] private float spdMultiplier;

    public Type UnitType { get => unitType; }

    public float SpdMultiplier { get => spdMultiplier; }
}
public class TerrainFloor : MonoBehaviour
{
    [SerializeField] private List<SpdMultiplierPerUnit> L_SpdMultiplierPerUnit = new List<SpdMultiplierPerUnit>();

    public float GetSpdMultiplierFromType(Type unitType)
    {
        return L_SpdMultiplierPerUnit.Find(unit => unit.UnitType == unitType).SpdMultiplier;
    }
}

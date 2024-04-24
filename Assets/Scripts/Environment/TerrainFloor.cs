using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum TerrainType
{
    Forest,
    Swamp,
    River,
    Plain
}

[Serializable]
public class EnvironnmentalBuffPerUnity
{
    [SerializeField] private Type unitType;
    [SerializeField] private int attackBonus;
    [SerializeField] private int defenseBonus;
    
    public Type UnitType { get => unitType; }
    public int AttackBonus { get => attackBonus; }
    public int DefenseBonus { get => defenseBonus; }
}


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
    [SerializeField] private TerrainType terrainType; 
    [SerializeField] private List<SpdMultiplierPerUnit> L_SpdMultiplierPerUnit = new List<SpdMultiplierPerUnit>();
    [SerializeField] private List<EnvironnmentalBuffPerUnity> L_EnvironnmentalBuffPerUnit = new List<EnvironnmentalBuffPerUnity>();

    public float GetSpdMultiplierFromType(Type unitType)
    {
        return L_SpdMultiplierPerUnit.Find(unit => unit.UnitType == unitType).SpdMultiplier;
    }
    
    public (int attack,int armor) GetBonusStatsFromType(Type unitType)
    {
        EnvironnmentalBuffPerUnity buff = L_EnvironnmentalBuffPerUnit.Find(unit => unit.UnitType == unitType);
        return buff != null ? (buff.AttackBonus, buff.DefenseBonus) : (0,0);
    }

    public bool DoesSpdModifierContainType(Type unitType)
    {
        return L_SpdMultiplierPerUnit.Find(unit => unit.UnitType == unitType) != null;
    }
    
    public bool DoesBuffModifierContainType(Type unitType)
    {
        return L_EnvironnmentalBuffPerUnit.Find(unit => unit.UnitType == unitType) != null;
    }
    
}

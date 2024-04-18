using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSetup", menuName = "ScriptableObjects/GameSetup", order = 1)]
public class SOGameSetup : ScriptableObject
{
    [SerializeField]
    private List<HouseToBuild> L_Buildings = new List<HouseToBuild>();

    [SerializeField] private SOGameGoal gameGoal;

    public SOGameGoal GameGoal { get => gameGoal; }
}

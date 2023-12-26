using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostType", menuName = "GhostType")]
public class GhostType : ScriptableObject
{
    public string typeName = "GhostName";

    [Range(0, 1f)]
    public float chance = 0.5f;
    [Range(0, 1f)]
    public float agrasiveness = 1;
    public float actionsDelay = 30000;
    public float inCompanyFactor = 1;
    public float elctricObjectFactor = 1;
    public float playersObjectFactor = 1;
    public float actionRange = 5;
    public float throwForce = 1;
    public Evidence[] evidences;
    public Peculiarity[] peculiarities;

}

using UnityEngine;

[CreateAssetMenu(fileName = "GhostPropereties", menuName = "Ghosts/GhostPropereties")]
public class GhostPropereties : ScriptableObject
{
    public string typeName = "GhostName";
    [Range(0, 1f)]
    public float chance,
    agrasiveness,
    chanceChangeFavoriteRoom,
    inCompanyFactor,
    electricObjectFactor,
    playersObjectFactor = 1;
    public float tempSpeed = 1;
    public float minTempInCurrentRoom = 8;
    public float minTempInFavoriteRoom = 4;

    public float actionsDelay = 30000;
    public float actionRange = 5;
    public float throwForce = 5;
}

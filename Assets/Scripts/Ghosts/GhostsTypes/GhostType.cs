using UnityEngine;

[CreateAssetMenu(fileName = "GhostType", menuName = "Ghosts/GhostType")]
public class GhostType : ScriptableObject
{
    [SerializeField] private GhostPropereties properety;
    public Evidence[] evidences;
    public Peculiarity[] peculiarities;

    public void Init()
    {

    }

    public GhostPropereties GetGhostPropereties()
    {
        return properety;
    }
}

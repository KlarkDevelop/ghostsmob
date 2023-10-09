using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    public Room[] rooms;
    private void Start()
    {
        rooms = gameObject.GetComponentsInChildren<Room>();
    }
}

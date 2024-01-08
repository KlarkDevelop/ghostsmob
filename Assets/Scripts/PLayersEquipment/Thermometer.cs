using UnityEngine;
public class Thermometer : ScannerEquipment
{
    private Room currentRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Room>(out Room room))
        {
            currentRoom = room;
        }
    }

    protected override void DoScan()
    {
        if (currentRoom != null) Debug.Log($"Therm {this}: In {currentRoom} Temp {currentRoom.temperature}"); //TODO: отображение температуры на приборе
    }
}

using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    public static Room[] rooms;
    [SerializeField] private float maxTemperature = 20; //TODO: разброс по температуре для большей реалистичности(поле разброса и подсчет итоговой температуры в методе ChangeRoomsTemperature) так же возможно стоит будет сделать разброс и у призрака
    [SerializeField] private float minTemperature = 8;
    public bool isElectricityOn = true;
    [SerializeField] private float tempSpeed;
    private void Awake()
    {
        rooms = gameObject.GetComponentsInChildren<Room>();
    }

    private void Update()
    {
        ChangeRoomsTemperature();
        if (Input.GetKeyDown(KeyCode.T)) isElectricityOn = !isElectricityOn; //TODO: логика электрощитка
    }

    private void ChangeRoomsTemperature()
    {
        if (isElectricityOn)
        {
            foreach (Room r in rooms)
            {
                r.ChangeTemperature(maxTemperature, tempSpeed);
            }
        }
        else
        {
            foreach (Room r in rooms)
            {
                r.ChangeTemperature(minTemperature, tempSpeed);
            }
        }
    }

    public static Room GetRandomRoom()
    {
        return rooms[UnityEngine.Random.Range(0, rooms.Length)];
    }

    public static Room FoundRoomOnPoint(Vector3 point)
    {
        int layerRoomsId = LayerMask.GetMask("Room");
        Collider[] col = Physics.OverlapSphere(point, 0.1f, layerRoomsId);

        if (col.Length != 0 && !col[0].gameObject.TryGetComponent<Room>(out Room foundedRoom))
        {
            return foundedRoom;
        }
        else
        {
            Debug.Log($"Error! Heres ara no rooms.");
            return null;
        }
    }
}

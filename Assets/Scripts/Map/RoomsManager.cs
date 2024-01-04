using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    [HideInInspector] public Room[] rooms;
    [SerializeField] private float maxTemperature = 20; //TODO: разброс по температуре для большей реалистичности(поле разброса и подсчет итоговой температуры в методе ChangeRoomsTemperature) так же возможно стоит будет сделать разброс и у призрака
    [SerializeField] private float minTemperature = 8;
    public bool isElectricityOn = true;
    public float tempGlobalSpeed = 1;
    [SerializeField] private float tempChangingSpeed = 1;
    private float tempElectricSpeed;
    private void Awake()
    {
        rooms = gameObject.GetComponentsInChildren<Room>();
        tempElectricSpeed = tempGlobalSpeed * tempChangingSpeed;
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
                r.ChangeTemperature(maxTemperature, tempElectricSpeed);
            }
        }
        else
        {
            foreach (Room r in rooms)
            {
                r.ChangeTemperature(minTemperature, tempElectricSpeed);
            }
        }
    }
}

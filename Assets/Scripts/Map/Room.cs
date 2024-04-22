using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform[] patrolPoints;
    public bool isLightOn;
    public float temperature = 10;
    public void ChangeTemperature(float goalTemp, float tempSpeed)
    {
        temperature = Mathf.MoveTowards(temperature, goalTemp, GameManager.settings.tempSpeed * tempSpeed * Time.deltaTime);
    }
}

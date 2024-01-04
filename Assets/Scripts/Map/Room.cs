using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float temperature = 10;
    public void ChangeTemperature(float goalTemp, float tempSpeed)
    {
        temperature = Mathf.MoveTowards(temperature, goalTemp, tempSpeed * Time.deltaTime);
    }
}

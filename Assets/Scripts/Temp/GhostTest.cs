using UnityEngine;
using UnityEngine.AI;

//https://youtu.be/-ctJjlZl2s8
public class AnimalAI : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private float movementSpeed;

    [SerializeField] private float changePositionTime = 5f;
    [SerializeField] private float moveDistance = 10f;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = movementSpeed;
        InvokeRepeating(nameof(MoveAnimal), changePositionTime, changePositionTime);
    }

    Vector3 RandomNavSphere(float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

        return navHit.position;
    }

    private void MoveAnimal()
    {
        _navMeshAgent.SetDestination(RandomNavSphere(moveDistance));
    }
}
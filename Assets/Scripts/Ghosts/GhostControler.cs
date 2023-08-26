using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GhostControler : MonoBehaviour
{
    private GhostState currentState = new PatrolRoomState();

    private NavMeshAgent Ai;

    public Room currentRoom;
    private int layerRoomsId;

    [HideInInspector] public bool isArrived;

    private void Start()
    {
        Ai = GetComponent<NavMeshAgent>();

        layerRoomsId = LayerMask.GetMask("Room");
        StartCoroutine(CheckRoom());

        currentState.Init(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ChangeState(new HuntState());
        currentState.Run();
        isArrived = Ai.remainingDistance <= Ai.stoppingDistance;
    }

    public void MoveTo(Vector3 pos)
    {
        Ai.SetDestination(pos);
    }

    private void ChangeState(GhostState state)
    {
        currentState = state;
        currentState.Init(this);
    }

    IEnumerator CheckRoom()
    {
        while (true)
        {
            Collider[] col = Physics.OverlapSphere(transform.position, 0.1f, layerRoomsId);

            if (col[0].gameObject.TryGetComponent<Room>(out Room foundedRoom))
            {
                if (currentRoom.roomId != foundedRoom.roomId)
                {
                    currentRoom = foundedRoom;
                    Debug.Log("Room changed");
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostControler : MonoBehaviour
{
    private iGhostState currentState = new HuntState();
    private NavMeshAgent Ai;

    public Room currentRoom;
    private int layerRoomsId;

    [HideInInspector] public bool isArrived;

    private void Start()
    {
        Ai = GetComponent<NavMeshAgent>();

        layerRoomsId = LayerMask.GetMask("Room");
        StartCoroutine(CheckRoom());
        StartCoroutine(ChekView());

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

    private void ChangeState(iGhostState state)
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

    [Header("Vision")]
    public float viewDist = 1;
    [Range(0, 360)]
    public float viewAngle = 60;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [SerializeField] private WaitForSeconds wait = new WaitForSeconds(0.2f);
    public Collider[] visibleTargets;
    public Collider nearestTarget;

    private Collider[] GetVisibleTargets()
    {
        Collider[] targetsInArea = Physics.OverlapSphere(transform.position, viewDist, targetMask);
        if (targetsInArea.Length != 0)
        {
            List<Collider> _visibleTargets = new List<Collider>();
            foreach (Collider target in targetsInArea)
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                        _visibleTargets.Add(target);
                }
            }

            if (_visibleTargets.Count > 0)
            {
                return _visibleTargets.ToArray();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private Collider GetNearestTarget()
    {
        if (visibleTargets != null)
        {
            Collider target = visibleTargets[0];
            foreach (Collider anotherTarget in visibleTargets)
            {
                if (Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(anotherTarget.transform.position, transform.position))
                {
                    target = anotherTarget;
                }
            }
            return target;
        }
        else
        {
            return null;
        }
    }

    IEnumerator ChekView()
    {
        while (true)
        {
            yield return wait;
            visibleTargets = GetVisibleTargets();
            nearestTarget = GetNearestTarget();
        }
    }
}

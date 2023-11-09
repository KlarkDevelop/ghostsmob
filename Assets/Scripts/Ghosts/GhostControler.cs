using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class GhostControler : NetworkBehaviour
{
    private iGhostState currentState = new PatrolRoomState();
    private NavMeshAgent Ai;

    public Room currentRoom;
    private int layerRoomsId;
    private int layerItemsId;

    [HideInInspector] public bool isArrived = false;

    [SerializeField] private RoomsManager allRooms;

    private void Start()
    {
        Ai = GetComponent<NavMeshAgent>();

        layerRoomsId = LayerMask.GetMask("Room");
        layerPlayersId = LayerMask.GetMask("Players");
        layerObstacleId = LayerMask.GetMask("Obstacle");
        layerItemsId = LayerMask.GetMask("Item");

        StartCoroutine(ChekView());
        StartCoroutine(CheckRoom());

        currentState.Init(this);

        NetworkManager.OnClientStarted += DisableControler;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) ChangeState(new HuntState());
        currentState.Run();
        if (Input.GetKeyDown(KeyCode.Space)) TryDoAction();
        isArrived = Ai.remainingDistance <= Ai.stoppingDistance;
    }

    public void MoveTo(Vector3 pos)
    {
        Ai.SetDestination(pos);
    }

    public void RandomMove()
    {
        Room randomRoom = allRooms.rooms[UnityEngine.Random.Range(0, allRooms.rooms.Length)];
        Vector3 randomPoint = randomRoom.patrolPoints[UnityEngine.Random.Range(0, randomRoom.patrolPoints.Length)].position;

        MoveTo(randomPoint);
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

            if (col.Length != 0 && col[0].gameObject.TryGetComponent<Room>(out Room foundedRoom))
            {
                if (currentRoom == null || currentRoom != foundedRoom)
                {
                    currentRoom = foundedRoom;
                    Debug.Log($"Ghost moved to another room! Now it in room: {currentRoom}");
                }
            }
            else
            {
                currentRoom = null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    [Header("Actions")]
    //Это будет в особеностях призрака
    [Range(0, 1f)]
    public float chance = 0.5f;
    public float agrasiveness = 1;
    public float actionsDelay = 30000;
    public float inCompanyFactor = 1;
    public float elctricObjectFactor = 1;
    public float playersObjectFactor = 1;
    //Это будет в особеностях призрака

    public float actionRange = 5;
    public float throwForce = 1;

    private void TryDoAction()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, actionRange, layerItemsId);
        if (col.Length != 0)
        {
            GameObject item = col[UnityEngine.Random.Range(0, col.Length)].gameObject;

            //TODO: проверка на то какой это предмет для высчета шанса взаимодействия

            DoAction(item);
        }
    }

    public void DoAction(GameObject obj)
    {
        Rigidbody item = obj.GetComponent<Rigidbody>();
        Collider[] players = Physics.OverlapSphere(transform.position, viewDist, layerPlayersId); //TODO: бросаться не в ближайших а в тех кто находитстья в той же комнате

        Vector3 throwVector = new Vector3();
        if (UnityEngine.Random.Range(0f, 1) <= agrasiveness && players.Length != 0)
        {
            Collider nearestPlayer = players[0];
            foreach (Collider anotherPlayer in players)
            {
                if (Vector3.Distance(nearestPlayer.transform.position, transform.position) > Vector3.Distance(anotherPlayer.transform.position, transform.position))
                {
                    nearestPlayer = anotherPlayer;
                }
            }
            throwVector = (nearestPlayer.transform.position - item.transform.position).normalized;
        }
        else
        {
            throwVector = new Vector3(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(0f, 1));
        }
        Vector3 throwDirection = throwVector * throwForce;

        item.AddForce(throwDirection, ForceMode.Impulse);
    }

    [Header("Vision")]
    public bool showActionGizmos = true;
    public bool showVisionGizmos = true;
    public float viewDist = 1;
    [Range(0, 360)]
    public float viewAngle = 60;
    public float viewOffset = 0;
    private LayerMask layerPlayersId;
    private LayerMask layerObstacleId;
    private WaitForSeconds wait = new WaitForSeconds(0.2f);
    public Collider[] visibleTargets;
    public Collider nearestTarget;

    private Collider[] GetVisibleTargets()
    {
        Vector3 startViewPoint = transform.position + new Vector3(0, viewOffset, 0);
        Collider[] targetsInArea = Physics.OverlapSphere(startViewPoint, viewDist, layerPlayersId);

        if (targetsInArea.Length != 0)
        {
            List<Collider> _visibleTargets = new List<Collider>();
            foreach (Collider target in targetsInArea)
            {
                Vector3 directionToTarget = (target.transform.position - startViewPoint).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(startViewPoint, target.transform.position);

                    if (!Physics.Raycast(startViewPoint, directionToTarget, distanceToTarget, layerObstacleId))
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

    private void DisableControler()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class GhostControler : NetworkBehaviour
{
    private iGhostState currentState = new PatrolRoomState();
    private NavMeshAgent Ai;
    public GhostType ghost;
    [HideInInspector] public GhostPropereties _ghostPropereties;
    public Room currentRoom;
    public Room favoriteRoom;
    private int layerRoomsId;
    private int layerItemsId;

    [HideInInspector] public bool isArrived = false;

    [SerializeField] private RoomsManager allRooms;

    private void Awake()
    {
        Ai = GetComponent<NavMeshAgent>();
        if (favoriteRoom == null) ChooseFavoriteRoom();
        GetCurrentRoom();
        ghost.Init();
        _ghostPropereties = ghost.GetGhostPropereties();
        currentState.Init(this);
    }

    private void Start()
    {
        layerRoomsId = LayerMask.GetMask("Room");
        layerPlayersId = LayerMask.GetMask("Players");
        layerObstacleId = LayerMask.GetMask("Obstacle");
        layerItemsId = LayerMask.GetMask("interactableObject");

        tempSpeed = _ghostPropereties.tempSpeed * allRooms.tempGlobalSpeed;

        StartCoroutine(ChekView());
        StartCoroutine(CheckRoom());

        NetworkManager.OnClientStarted += DisableControler;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F)) ChangeState(new HuntState());
        currentState.Run();
        if (Input.GetKeyDown(KeyCode.Space)) TryDoAction();
        isArrived = Ai.remainingDistance <= Ai.stoppingDistance;

        InfluenceOnTemperature();
        InfluenceOnTemperatureInFavoriteRoom();
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

    private void ChooseFavoriteRoom()
    {
        favoriteRoom = allRooms.rooms[Random.Range(0, allRooms.rooms.Length)];
    }

    IEnumerator CheckRoom()
    {
        while (true)
        {
            GetCurrentRoom();

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void GetCurrentRoom()
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
    }

    private void TryDoAction()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, _ghostPropereties.actionRange, layerItemsId);
        if (col.Length != 0)
        {
            GameObject item = col[UnityEngine.Random.Range(0, col.Length)].gameObject;

            //TODO: проверка на то какой это предмет для высчета шанса взаимодействия (передача в DoAction переменной с типом класса для роботы конеретной перегрузки)

            DoAction(item);
        }
    }

    [SerializeField]
    private GameObject EMPSource;
    public static UnityEvent<EMPSignalSource> onAction = new UnityEvent<EMPSignalSource>();
    public void DoAction(GameObject obj) //TODO: разделить логику в зависимости от того что это за предмет (перегрузкой метода) и зарефакторить этот метод
    {
        Rigidbody item = obj.GetComponent<Rigidbody>();

        List<Transform> players = new List<Transform>();

        foreach (PlayerControler player in PlayersManager.Singleton.players)
        {
            if (currentRoom == player.currentRoom)
            {
                players.Add(player.transform);
            }
        }

        Vector3 throwVector = new Vector3();
        if (UnityEngine.Random.Range(0f, 1) <= _ghostPropereties.agrasiveness && players.Count != 0)
        {
            Transform nearestPlayer = players[0];
            foreach (Transform anotherPlayer in players)
            {
                if (Vector3.Distance(nearestPlayer.position, transform.position) > Vector3.Distance(anotherPlayer.position, transform.position))
                {
                    nearestPlayer = anotherPlayer;
                }
            }
            throwVector = (nearestPlayer.position - item.transform.position).normalized;
        }
        else
        {
            throwVector = new Vector3(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(0f, 1), UnityEngine.Random.Range(-1f, 1));
        }
        Vector3 throwDirection = throwVector * _ghostPropereties.throwForce;

        item.AddForce(throwDirection, ForceMode.Impulse);
        EMPSignalSource source;
        if (item.TryGetComponent<EMPSignalSource>(out source))
        {
            source.UpdateTimer();
        }
        else
        {
            source = Instantiate(EMPSource, item.transform).GetComponent<EMPSignalSource>();
        }
        onAction.Invoke(source);
    }

    private float tempSpeed;
    private void InfluenceOnTemperature()
    {
        if (currentRoom != favoriteRoom)
        {
            currentRoom.ChangeTemperature(_ghostPropereties.minTempInCurrentRoom, tempSpeed);
        }
        else
        {
            currentRoom.ChangeTemperature(_ghostPropereties.minTempInFavoriteRoom, tempSpeed * 2);
        }
    }

    private void InfluenceOnTemperatureInFavoriteRoom()
    {
        favoriteRoom.ChangeTemperature(_ghostPropereties.minTempInCurrentRoom, tempSpeed);
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

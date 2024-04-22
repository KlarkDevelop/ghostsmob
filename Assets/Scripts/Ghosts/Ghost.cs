using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : NetworkBehaviour
{
    [HideInInspector] public GhostPropereties propereties;
    private iGhostState currentState = new PatrolRoomState();
    public GhostType _ghostType;

    private NavMeshAgent Ai;
    private ActionController _actionController;
    [HideInInspector] public Vision _vision;

    public Room currentRoom { get; set; }
    public Room favoriteRoom;

    [HideInInspector] public bool isArrived = false;

    private void Awake()
    {
        Ai = GetComponent<NavMeshAgent>();
        if (favoriteRoom == null) ChooseFavoriteRoom();

        _ghostType.Init();

        propereties = _ghostType.GetGhostPropereties();
        currentState.Init(this);

        _actionController = GetComponent<ActionController>();
        _actionController.Init(this);

        _vision = GetComponent<Vision>();
        _vision.Init();
    }

    private void Start()
    {
        NetworkManager.OnClientStarted += OnClient;
        currentRoom = RoomsManager.FoundRoomOnPoint(transform.position);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F)) ChangeState(new HuntState());
        currentState.Run();
        if (Input.GetKeyDown(KeyCode.Space)) _actionController.TryDoAction();
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
        Room randomRoom = RoomsManager.GetRandomRoom();
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
        favoriteRoom = RoomsManager.GetRandomRoom();
    }

    private void InfluenceOnTemperature()
    {
        if (currentRoom != favoriteRoom)
        {
            currentRoom.ChangeTemperature(propereties.minTempInCurrentRoom, propereties.tempSpeed);
        }
        else
        {
            currentRoom.ChangeTemperature(propereties.minTempInFavoriteRoom, propereties.tempSpeed);
        }
    }

    private void InfluenceOnTemperatureInFavoriteRoom()
    {
        favoriteRoom.ChangeTemperature(propereties.minTempInCurrentRoom, propereties.tempSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Room>(out Room foundedRoom))
        {
            if (currentRoom == null || currentRoom != foundedRoom)
            {
                currentRoom = foundedRoom;
                Debug.Log($"Ghost moved to room: {currentRoom}");
            }
        }
    }

    private void OnClient()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }
}

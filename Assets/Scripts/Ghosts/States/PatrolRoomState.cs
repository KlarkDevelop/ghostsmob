using System;
using UnityEngine;

public class PatrolRoomState : iGhostState
{
    public static event Action onStart;
    public GhostControler _ghostCont { get; set; }
    private float standTime = 2f;
    private float resetStT;
    private Room patrolingRoom;
    public void Init(GhostControler ghost)
    {
        _ghostCont = ghost;
        resetStT = standTime;
        patrolingRoom = _ghostCont.favoriteRoom;

        onStart?.Invoke();
    }

    public void Run()
    {
        PatrolMove();
    }

    private void PatrolMove()
    {
        if (_ghostCont.isArrived)
        {
            standTime -= Time.deltaTime;
            if (standTime <= 0)
            {
                _ghostCont.MoveTo(patrolingRoom.patrolPoints[
                            UnityEngine.Random.Range(0, patrolingRoom.patrolPoints.Length)].position);
                standTime = resetStT;
            }
        }
    }
}

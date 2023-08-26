using System;
using UnityEngine;

public class PatrolRoomState : GhostState
{
    public static event Action onStart;
    public GhostControler _ghostCont { get; set; }
    private float standTime = 2f;
    private float resetStT;
    public override void Init(GhostControler ghost)
    {
        _ghostCont = ghost;
        resetStT = standTime;
        _ghostCont.MoveTo(_ghostCont.currentRoom.patrolPoints[0].position);

        onStart?.Invoke();
    }

    public override void Run()
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
                _ghostCont.MoveTo(_ghostCont.currentRoom.patrolPoints[
                            UnityEngine.Random.Range(0, _ghostCont.currentRoom.patrolPoints.Length)].position);
                standTime = resetStT;
            }
        }
    }
}
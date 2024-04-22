using System;
using UnityEngine;

public class PatrolRoomState : iGhostState
{
    public static event Action onStart;
    public Ghost _ghost { get; set; }
    private float standTime = 2f;
    private float resetStT;
    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        resetStT = standTime;

        onStart?.Invoke();
    }

    public void Run()
    {
        PatrolMove();
    }

    private void PatrolMove()
    {
        if (_ghost.isArrived)
        {
            standTime -= Time.deltaTime;
            if (standTime <= 0)
            {
                _ghost.MoveTo(_ghost.favoriteRoom.patrolPoints[
                            UnityEngine.Random.Range(0, _ghost.favoriteRoom.patrolPoints.Length)].position);
                standTime = resetStT;
            }
        }
    }
}

using System;

public class HuntState : iGhostState
{
    public static event Action onStart;
    public Ghost _ghost { get; set; }

    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        onStart?.Invoke();
        _ghost.RandomMove();
    }

    public void Run()
    {
        Hunt();
    }

    private void Hunt()
    {
        if (_ghost._vision.nearestTarget != null)
        {
            _ghost.MoveTo(_ghost._vision.nearestTarget.transform.position);
        }
        else
        {
            if (_ghost.isArrived)
            {
                _ghost.RandomMove();
            }
        }
    }
}
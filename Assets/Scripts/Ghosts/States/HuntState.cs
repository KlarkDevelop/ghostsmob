using System;

public class HuntState : iGhostState
{
    public static event Action onStart;
    public GhostControler _ghostCont { get; set; }
    public void Init(GhostControler ghost)
    {
        _ghostCont = ghost;
        onStart?.Invoke();
        _ghostCont.RandomMove();
    }

    public void Run()
    {
        if (_ghostCont.nearestTarget != null)
        {
            _ghostCont.MoveTo(_ghostCont.nearestTarget.transform.position);
        }
        else
        {
            if (_ghostCont.isArrived)
            {
                _ghostCont.RandomMove();
            }
        }
    }
}
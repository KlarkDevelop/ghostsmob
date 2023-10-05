using System;

public class HuntState : iGhostState
{
    public static event Action onStart;
    public GhostControler _ghostCont { get; set; }
    public void Init(GhostControler ghost)
    {
        onStart?.Invoke();
    }

    public void Run()
    {

    }
}
using System;

public interface iGhostState
{
    public GhostControler _ghostCont { get; set; }
    public void Init(GhostControler ghost);
    public void Run();
}

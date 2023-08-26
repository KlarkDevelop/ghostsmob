public abstract class GhostState
{
    GhostControler _ghostCont { get; set; }

    public virtual void Init(GhostControler ghost) { }

    public abstract void Run();
}

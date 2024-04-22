public interface iGhostState
{
    public Ghost _ghost { get; set; }
    public void Init(Ghost ghost);
    public void Run();
}

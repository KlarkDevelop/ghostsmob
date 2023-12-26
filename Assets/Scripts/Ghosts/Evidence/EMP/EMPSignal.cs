public class EMPSignal : Evidence
{
    public override void Init()
    {
        GhostControler.onAction.AddListener(SetMaxSignal);
    }

    private void SetMaxSignal(EMPSignalSource signalArea)
    {
        signalArea.value = 5;
    }
}

using UnityEngine;
[CreateAssetMenu(fileName = "EMPasset", menuName = "Evidence/EMP")]
public class EMPSignal : Evidence
{
    public override void Init()
    {
        ActionController.onAction.AddListener(SetMaxSignal);
    }

    private void SetMaxSignal(EMPSignalSource signalArea)
    {
        signalArea.value = 5;
    }
}

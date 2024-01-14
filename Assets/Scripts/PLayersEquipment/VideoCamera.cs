using UnityEngine;

public class VideoCamera : PlayerEquipment, iToggleable
{
    [SerializeField] private GameObject outputScrean;
    [SerializeField] private Camera inputCamera;
    private void Start()
    {
        ToggleVideoCamera(isTurnOn);
    }

    public void Toggl()
    {
        isTurnOn = !isTurnOn;
        ToggleVideoCamera(isTurnOn);
    }

    private void ToggleVideoCamera(bool state)
    {
        outputScrean.SetActive(state);
        inputCamera.gameObject.SetActive(state);
    }

}

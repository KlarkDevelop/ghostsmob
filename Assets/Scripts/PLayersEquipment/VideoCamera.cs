using UnityEngine;

public class VideoCamera : PlayerEquipment, iToggleable
{
    [SerializeField] private GameObject outputScrean;
    [SerializeField] private Camera inputCamera;

    private RenderTexture render;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;
    private void Start()
    {
        render = new RenderTexture(width, height, 24);
        render.name = "CamTexture";
        inputCamera.targetTexture = render;
        ToggleVideoCamera(isTurnOn);

        outputScrean.GetComponent<Renderer>().material.SetTexture("_BaseMap", render);
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

    private void OnDisable()
    {
        if (render != null) render.Release();
    }

    private void OnDestroy()
    {
        if (render != null)
        {
            render.Release();
            Destroy(render);
        }
    }

}

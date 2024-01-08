using UnityEngine;
using System.Collections;

public abstract class ScannerEquipment : PlayerEquipment, iToggleable
{
    [SerializeField] private float frequency;
    private WaitForSeconds freq = new WaitForSeconds(1f);

    private void Start()
    {
        freq = new WaitForSeconds(frequency);
        if (isTurnOn)
        {
            StartCoroutine(DoItemScan());
        }
    }

    public void Toggl()
    {
        if (isTurnOn)
        {
            StartCoroutine(DoItemScan());
            isTurnOn = false;
        }
        else
        {
            StopCoroutine(DoItemScan());
            isTurnOn = true;
        }
    }

    private IEnumerator DoItemScan()
    {
        while (true)
        {
            DoScan();
            yield return freq;
        }
    }

    protected abstract void DoScan();

}

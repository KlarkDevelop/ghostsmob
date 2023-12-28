using System.Collections;
using UnityEngine;

public class EMP : MonoBehaviour, iToggleable
{
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask sourceMask;
    public bool isTurnOn = false;

    private void Start()
    {
        if (isTurnOn)
        {
            StartCoroutine(ScanArea());
        }
    }

    public void Toggl()
    {
        if (isTurnOn)
        {
            StopCoroutine(ScanArea());
            isTurnOn = false;
        }
        else
        {
            StartCoroutine(ScanArea());
            isTurnOn = true;
        }
    }

    private WaitForSeconds frequency = new WaitForSeconds(0.3f);
    private IEnumerator ScanArea()
    {
        while (true)
        {
            Collider[] sources = Physics.OverlapSphere(transform.position, range, sourceMask);
            if (sources.Length != 0)
            {
                Collider nearestSource = sources[0];
                foreach (Collider sourceCol in sources)
                {
                    if (Vector3.Distance(transform.position, nearestSource.transform.position) < Vector3.Distance(transform.position, sourceCol.transform.position))
                    {
                        nearestSource = sourceCol;
                    }
                }

                EMPSignalSource source = nearestSource.GetComponent<EMPSignalSource>();
                Debug.Log($"EMP: {source.value}"); //TODO: сделать отображение уровня ЭМП
            }
            yield return frequency;
        }
    }

    [SerializeField] private bool drawRange = false;
    private void OnDrawGizmosSelected()
    {
        if (drawRange)
        {
            Gizmos.color = Color.white;

            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}

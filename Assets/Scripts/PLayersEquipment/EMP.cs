using UnityEngine;

public class EMP : ScannerEquipment
{
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask sourceMask;
    protected override void DoScan()
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

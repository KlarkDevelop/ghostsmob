using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public bool showVisionGizmos = true;
    public float viewDist = 1;
    [Range(0, 360)]
    public float viewAngle = 60;
    public float viewOffset = 0;
    private LayerMask layerPlayersId;
    private LayerMask layerObstacleId;
    private WaitForSeconds wait = new WaitForSeconds(0.2f);
    public Collider[] visibleTargets;
    public Collider nearestTarget;

    public void Init()
    {
        layerPlayersId = LayerMask.GetMask("Players");
        layerObstacleId = LayerMask.GetMask("Obstacle");

        StartCoroutine(ChekView());
    }

    private Collider[] GetVisibleTargets()
    {
        Vector3 startViewPoint = transform.position + new Vector3(0, viewOffset, 0);
        Collider[] targetsInArea = Physics.OverlapSphere(startViewPoint, viewDist, layerPlayersId);

        if (targetsInArea.Length != 0)
        {
            List<Collider> _visibleTargets = new List<Collider>();
            foreach (Collider target in targetsInArea)
            {
                Vector3 directionToTarget = (target.transform.position - startViewPoint).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(startViewPoint, target.transform.position);

                    if (!Physics.Raycast(startViewPoint, directionToTarget, distanceToTarget, layerObstacleId))
                        _visibleTargets.Add(target);
                }
            }

            if (_visibleTargets.Count > 0)
            {
                return _visibleTargets.ToArray();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private Collider GetNearestVisibleTarget()
    {
        if (visibleTargets != null)
        {
            Collider target = visibleTargets[0];
            foreach (Collider anotherTarget in visibleTargets)
            {
                if (Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(anotherTarget.transform.position, transform.position))
                {
                    target = anotherTarget;
                }
            }
            return target;
        }
        else
        {
            return null;
        }
    }

    IEnumerator ChekView()
    {
        while (true)
        {
            yield return wait;
            visibleTargets = GetVisibleTargets();
            nearestTarget = GetNearestVisibleTarget();
        }
    }
}

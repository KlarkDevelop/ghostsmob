using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GhostControler))]
public class DebugGhostView : Editor
{
    private void OnSceneGUI()
    {
        GhostControler fov = (GhostControler)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewDist);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.viewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.viewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.viewDist);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.viewDist);

        if (fov.nearestTarget != null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.nearestTarget.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

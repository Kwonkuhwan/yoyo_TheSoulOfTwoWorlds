using UnityEngine;

public class CrosshairLine : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public float maxDistance = 100f;
    public LayerMask raycastLayers;

    void Update()
    {
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, maxDistance, raycastLayers))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = origin + direction * maxDistance;
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);
    }
}

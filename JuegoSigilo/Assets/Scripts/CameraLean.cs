using UnityEngine;

public class CameraLean : MonoBehaviour
{
    public Transform target;

    public float derechaMax;
    public float izquierdaMax;
    public float arribaMax;
    public float abajoMax;

    public float cameraHeight = 20f;

    private Camera cam;

    public Renderer planeRenderer;

    private void Awake() {
        cam = GetComponent<Camera>();
        if (cam == null || target == null || planeRenderer == null)
        {
            Debug.LogError("Asigna el Target y el Plane Renderer en el Inspector.");
            enabled = false;
            return;
        }

        transform.rotation = Quaternion.Euler(60f, 0f, 0f);

        Vector3 pos = transform.position;
        pos.y = cameraHeight;
        transform.position = pos;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        derechaMax = planeRenderer.bounds.max.x - halfWidth;
        izquierdaMax = planeRenderer.bounds.min.x + halfWidth;
        arribaMax = planeRenderer.bounds.max.z - halfHeight - 1.8f;
        abajoMax = planeRenderer.bounds.min.z + halfHeight - 2f;
    }

    private void LateUpdate() {
        if (target == null) return;

        float x = Mathf.Clamp(target.position.x, izquierdaMax, derechaMax);
        float z = Mathf.Clamp(target.position.z, abajoMax, arribaMax);

        transform.position = new Vector3(x, cameraHeight, z - 5f);
    }
}

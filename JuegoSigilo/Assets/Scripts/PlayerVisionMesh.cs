using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlayerVisionMesh : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float viewDistance = 10f;
    [SerializeField, Range(1f, 360f)] private float viewAngle = 90f;
    [SerializeField, Range(10, 200)] private int rayCount = 100;
    [SerializeField] private float eyeHeight = 1f;

    [Header("Layers")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask visionTargetMask;

    private Mesh visionMesh;

    private void Awake()
    {
        visionMesh = new Mesh();
        visionMesh.name = "Player Vision";

        GetComponent<MeshFilter>().sharedMesh = visionMesh;
    }

    private void LateUpdate()
    {
        GenerateVisionMesh();
        UpdateVisionTargets();
    }

    // =========================================================
    // VISION MESH
    // =========================================================

    private void GenerateVisionMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 origin = Vector3.up * eyeHeight;

        vertices.Add(origin);

        float angleStep = viewAngle / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -viewAngle / 2f + angleStep * i;

            Vector3 direction = DirectionFromAngle(angle);

            Vector3 worldOrigin = transform.TransformPoint(origin);
            Vector3 worldDirection = transform.TransformDirection(direction);

            float distance = viewDistance;

            if (Physics.Raycast(
                worldOrigin,
                worldDirection,
                out RaycastHit hit,
                viewDistance,
                obstacleMask))
            {
                distance = hit.distance;
            }

            Vector3 localPoint = origin + direction * distance;

            vertices.Add(localPoint);

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        visionMesh.Clear();

        visionMesh.SetVertices(vertices);
        visionMesh.SetTriangles(triangles, 0);

        visionMesh.RecalculateNormals();
        visionMesh.RecalculateBounds();
    }

    // =========================================================
    // VISION TARGETS
    // =========================================================

    private void UpdateVisionTargets()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            viewDistance,
            visionTargetMask
        );

        foreach (Collider target in targets)
        {
            Renderer targetRenderer = target.GetComponentInChildren<Renderer>();

            if (targetRenderer == null)
                continue;

            bool visible = IsTargetVisible(target);

            targetRenderer.enabled = visible;
        }
    }

    private bool IsTargetVisible(Collider target)
    {
        Vector3 origin = transform.position;
        origin.y += eyeHeight;

        // Punto del objetivo al que miramos.
        Vector3 targetPoint = target.bounds.center;

        Vector3 direction = targetPoint - origin;

        float distance = direction.magnitude;

        // Está fuera de nuestra distancia de visión.
        if (distance > viewDistance)
            return false;

        // Comprobamos el ángulo.
        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > viewAngle / 2f)
            return false;

        // Comprobamos si hay una pared.
        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            distance,
            obstacleMask))
        {
            // Hay un obstáculo entre nosotros y el objetivo.
            return false;
        }

        return true;
    }

    // =========================================================
    // DIRECCION
    // =========================================================

    private Vector3 DirectionFromAngle(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Sin(radians),
            0f,
            Mathf.Cos(radians)
        );
    }
}
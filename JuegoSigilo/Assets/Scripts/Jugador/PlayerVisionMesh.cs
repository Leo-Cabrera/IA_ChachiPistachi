
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlayerVisionMesh : MonoBehaviour
{
    [SerializeField] private float viewDistance = 10f;
    [SerializeField, Range(1f, 360f)] private float viewAngle = 90f;
    [SerializeField, Range(10, 200)] private int rayCount = 100;
    [SerializeField] private float eyeHeight = 1f;

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask visionTargetMask;

    // Enemigos que eran visibles por el cono en el frame anterior
    private List<RefEnemy> previousTargets = new List<RefEnemy>();

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

    private void UpdateVisionTargets()
    {
        // 1. Los que eran visibles el frame anterior
        // dejan de estar visibles por el cono.
        foreach (RefEnemy enemy in previousTargets)
        {
            if (enemy == null)
                continue;

            enemy.coneVisibility = false;
            enemy.changeVisibility();
        }

        // 2. Buscamos los enemigos que están dentro del alcance.
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            viewDistance,
            visionTargetMask
        );

        // 3. Creamos la lista de enemigos visibles en ESTE frame.
        List<RefEnemy> currentTargets = new List<RefEnemy>();

        foreach (Collider target in targets)
        {
            RefEnemy refEnemy = target.GetComponentInChildren<RefEnemy>();

            if (refEnemy == null)
                continue;

            bool visible = IsTargetVisible(target);

            // Si está dentro del cono y no hay obstáculos,
            // pertenece a los objetivos actuales.
            if (visible)
            {
                refEnemy.coneVisibility = true;
                refEnemy.changeVisibility();

                if (!currentTargets.Contains(refEnemy))
                {
                    currentTargets.Add(refEnemy);
                }
            }
        }

        // 4. Guardamos los objetivos actuales para el siguiente frame.
        previousTargets = currentTargets;
    }

    private bool IsTargetVisible(Collider target)
    {
        Vector3 origin = transform.position;
        origin.y += eyeHeight;

        Vector3 targetPoint = target.bounds.center;

        Vector3 direction = targetPoint - origin;

        float distance = direction.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > viewAngle / 2f)
            return false;

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            distance,
            obstacleMask))
        {
            return false;
        }

        return true;
    }

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

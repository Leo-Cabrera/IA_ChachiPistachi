// using UnityEngine;

// public class PlayerRaycast : MonoBehaviour
// {
//     [SerializeField] private float visionDistance = 10f;
//     [SerializeField] private float visionAngle = 90f;

//     [SerializeField] private LayerMask obstacleMask;
//     [SerializeField] private LayerMask targetMask;

//     private void Update()
//     {
//         DetectObjects();
//     }

//     private void DetectObjects()
//     {
//         Collider[] objectsInRange = Physics.OverlapSphere(
//             transform.position,
//             visionDistance,
//             targetMask
//         );

//         foreach (Collider target in objectsInRange)
//         {
//             Vector3 directionToTarget = target.transform.position - transform.position;
//             directionToTarget.y = 0f;

//             float angle = Vector3.Angle(transform.forward, directionToTarget);

//             // Fuera del cono de vision
//             if (angle > visionAngle / 2f)
//                 continue;

//             float distance = directionToTarget.magnitude;

//             // Comprobamos si hay una pared entre el jugador y el objeto
//             if (Physics.Raycast(
//                 transform.position,
//                 directionToTarget.normalized,
//                 distance,
//                 obstacleMask))
//             {
//                 // Hay una pared bloqueando la vision
//                 Debug.DrawRay(
//                     transform.position,
//                     directionToTarget,
//                     Color.red
//                 );

//                 continue;
//             }

//             // El objeto es visible
//             Debug.DrawRay(
//                 transform.position,
//                 directionToTarget,
//                 Color.green
//             );

//             Debug.Log("Viendo: " + target.name);
//         }
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.yellow;

//         Gizmos.DrawWireSphere(
//             transform.position,
//             visionDistance
//         );

//         Vector3 leftBoundary =
//             Quaternion.Euler(0, -visionAngle / 2f, 0) *
//             transform.forward;

//         Vector3 rightBoundary =
//             Quaternion.Euler(0, visionAngle / 2f, 0) *
//             transform.forward;

//         Gizmos.DrawRay(
//             transform.position,
//             leftBoundary * visionDistance
//         );

//         Gizmos.DrawRay(
//             transform.position,
//             rightBoundary * visionDistance
//         );
//     }
// }
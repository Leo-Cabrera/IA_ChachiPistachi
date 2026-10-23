using UnityEngine;
using System.Collections.Generic;

public class ZombieBossPath : MonoBehaviour
{
    [Header("Ruta del jefe")]
    public Transform[] puntosRuta;

    [Header("Movimiento")]
    public float velocidad = 1.5f;
    public float rotacionVel = 5f;
    public float distanciaPunto = 0.3f;

    private Parilla parilla;
    private List<Nodo> camino;
    private int indiceCamino = 0;
    private int indicePuntoActual = 0;

    private void Start()
    {
        // Busca en la escena un objeto que tenga el componente Parilla
        parilla = FindFirstObjectByType<Parilla>();

        // Muestra un error si no se encuentra la parrilla en la escena
        if (parilla == null)
            Debug.LogError("No se encuentra Parilla en la escena.");
    }

    private void Update()
    {

        // Comprueba que exista la parilla y que si hay puntos
        // Si falta alguno de estos elementos, no continúa el movimiento
        if (parilla == null || puntosRuta == null || puntosRuta.Length == 0)
            return;

        // Comprueba que el waypoint actual esta asignado
        if (puntosRuta[indicePuntoActual] == null)
            return;

        // Si no hay un camino, calcula uno
        // desde la posición del jefe hasta el waypoint actual
        if (camino == null || camino.Count == 0)
        {
            CalcularCaminoAlPunto();
            return;
        }

        // Comprueba si se han recorrido todos los nodos del camino
        if (indiceCamino >= camino.Count)
        {

            // Pasa al siguiente waypoint de la ruta
            // Si llega al ultimo, vuelve al primero
            indicePuntoActual = (indicePuntoActual + 1) % puntosRuta.Length;

            // Elimina el camino anterior para calcular uno nuevo
            camino = null;

            // Reinicia el indice del camino
            indiceCamino = 0;
            return;
        }

        // Obtiene la posicion del siguiente nodo del camino
        Vector3 destino = camino[indiceCamino].posicionMundo;
        // Calcula la dirección hacia ese nodo
        Vector3 direccion = destino - transform.position;
        //pa no moverse en el eje vertical, lo ponemos a 0
        direccion.y = 0f;

        // Comprueba si el jefe ya está suficientemente cerca del nodo
        if (direccion.magnitude <= distanciaPunto)
        {
            // Si ha llegado, pasa al siguiente nodo del camino
            indiceCamino++;
            return;
        }

        // Gira al jefe hacia el nodo
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);
        // Mueve al jefe hacia el nodo
        transform.position += direccion.normalized * velocidad * Time.deltaTime;
    }

    private void CalcularCaminoAlPunto()
    {
        Nodo inicio = parilla.ObtenerNodoDesdePosicion(transform.position);
        Nodo objetivo = parilla.ObtenerNodoDesdePosicion(puntosRuta[indicePuntoActual].position);

        camino = parilla.BuscarCamino(inicio, objetivo);
        indiceCamino = 0;

        if (camino == null || camino.Count == 0)
            Debug.LogWarning("El jefe no encuentra una ruta al siguiente waypoint.");
    }

    
private void OnDrawGizmos()
{


    
    // Dibujamos el camino calculado por A*.
    if (camino != null)
    {
        for (int i = 0; i < camino.Count; i++)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(camino[i].posicionMundo, 0.15f);

            if (i < camino.Count - 1)
            {
                Gizmos.DrawLine(
                    camino[i].posicionMundo,
                    camino[i + 1].posicionMundo
                );
            }
        }
    }

    if (puntosRuta == null || puntosRuta.Length == 0)
        return;

    for (int i = 0; i < puntosRuta.Length; i++)
    {
        if (puntosRuta[i] == null)
            continue;

        // Dibujamos una esfera en cada waypoint.
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(puntosRuta[i].position, 0.3f);

        // Mostramos el número del waypoint en la Scene.
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            puntosRuta[i].position + Vector3.up * 0.5f,
            "Punto " + i
        );
        #endif

        // Unimos cada punto con el siguiente.
        if (i < puntosRuta.Length - 1 && puntosRuta[i + 1] != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                puntosRuta[i].position,
                puntosRuta[i + 1].position
            );
        }
    }
}

}

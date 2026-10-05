using UnityEngine;
using System.Collections.Generic;

public class Parilla : MonoBehaviour
{
    public Vector2 tamParrilla;
    public float tamNodo;

    public LayerMask capaObstaculos;

    private Nodo[,] nodos;
    
    // private Nodo nodoEnemigo;
    private Nodo nodoJugador;

    // private List<Nodo> vecinosEnemigo;

    //private List<Nodo> camino;

    // private float tiempoRecalculo = 0.2f;
    // private float contadorRecalculo;


    void Start()
    {
        CrearParilla();
    }

    void Update()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null && nodos != null)
        {
            nodoJugador = ObtenerNodoDesdePosicion(jugador.transform.position);
        }
    }

    void CrearParilla()
    {
        int cantidadNodosX = Mathf.RoundToInt(tamParrilla.x / tamNodo);
        int cantidadNodosY = Mathf.RoundToInt(tamParrilla.y / tamNodo);

        nodos = new Nodo[cantidadNodosX, cantidadNodosY];

        Vector3 esquinaInferior = transform.position - Vector3.right * tamParrilla.x / 2f - Vector3.forward * tamParrilla.y / 2f;

        for (int x = 0; x < cantidadNodosX; x++)
        {
            for (int y = 0; y < cantidadNodosY; y++)
            {
                Vector3 posicionMundo = esquinaInferior
                    + Vector3.right * (x * tamNodo + tamNodo / 2f)
                    + Vector3.forward * (y * tamNodo + tamNodo / 2f);

                bool caminable = !Physics.CheckSphere(posicionMundo, tamNodo / 2f, capaObstaculos);

                nodos[x, y] = new Nodo(caminable, posicionMundo, x, y);
            }
        }
    }

    public List<Nodo> ObtenerVecinos(Nodo nodo)
    {
        List<Nodo> vecinos = new List<Nodo>();

        if (nodo == null || nodos == null)
            return vecinos;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int vecinoX = nodo.posX + x;
                int vecinoY = nodo.posY + y;

                if (vecinoX >= 0 && vecinoX < nodos.GetLength(0) &&
                    vecinoY >= 0 && vecinoY < nodos.GetLength(1))
                {
                    if (nodos[vecinoX, vecinoY].caminable)
                    {
                        vecinos.Add(nodos[vecinoX, vecinoY]);
                    }
                }
            }
        }

        return vecinos;
    }


    public Nodo ObtenerNodoDesdePosicion(Vector3 posicionMundo)
    {
        float porcentajeX = (posicionMundo.x - (transform.position.x - tamParrilla.x / 2f)) / tamParrilla.x;
        float porcentajeY = (posicionMundo.z - (transform.position.z - tamParrilla.y / 2f)) / tamParrilla.y;

        porcentajeX = Mathf.Clamp01(porcentajeX);
        porcentajeY = Mathf.Clamp01(porcentajeY);

        int posX = Mathf.FloorToInt(porcentajeX * nodos.GetLength(0));
        int posY = Mathf.FloorToInt(porcentajeY * nodos.GetLength(1));

        posX = Mathf.Clamp(posX, 0, nodos.GetLength(0) - 1);
        posY = Mathf.Clamp(posY, 0, nodos.GetLength(1) - 1);

        return nodos[posX, posY];
    }

    void OnDrawGizmos()
    {
        if (nodos == null)
            return;

        foreach (Nodo nodo in nodos)
        {
            if (nodo == nodoJugador)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = nodo.caminable ? Color.green : Color.red;
            }

            Gizmos.DrawCube( nodo.posicionMundo, Vector3.one * (tamNodo * 0.9f));
        }
    }

    public int CalcularDistancia(Nodo nodoA, Nodo nodoB)
    {
        int distX = Mathf.Abs(nodoA.posX - nodoB.posX);
        int distY = Mathf.Abs(nodoA.posY - nodoB.posY);

        int distDiagonal = Mathf.Min(distX, distY);
        int distRecta = Mathf.Abs(distX - distY);

        return 14 * distDiagonal + 10 * distRecta;
    }

    public List<Nodo> BuscarCamino(Nodo inicio, Nodo objetivo)
    {
        // Reiniciamos los datos de todos los nodos
        // para que cada enemigo tenga su propio cálculo de A*
        foreach (Nodo nodo in nodos)
        {
            nodo.costeInicio = int.MaxValue;
            nodo.costeObjetivo = 0;
            nodo.padre = null;
        }

        List<Nodo> abiertos = new List<Nodo>();
        HashSet<Nodo> cerrados = new HashSet<Nodo>();

        inicio.costeInicio = 0;
        inicio.costeObjetivo = CalcularDistancia(inicio, objetivo);

        abiertos.Add(inicio);

        while (abiertos.Count > 0)
        {
            Nodo nodoActual = abiertos[0];

            for (int i = 1; i < abiertos.Count; i++)
            {
                if (abiertos[i].CosteTotal < nodoActual.CosteTotal || (abiertos[i].CosteTotal == nodoActual.CosteTotal && abiertos[i].costeObjetivo < nodoActual.costeObjetivo))
                {
                    nodoActual = abiertos[i];
                }
            }

            abiertos.Remove(nodoActual);
            cerrados.Add(nodoActual);

            if (nodoActual == objetivo)
            {
                return CrearCamino(inicio, objetivo);
            }

            foreach (Nodo vecino in ObtenerVecinos(nodoActual))
            {
                if (cerrados.Contains(vecino))
                    continue;

                int nuevoCosteInicio = nodoActual.costeInicio +
                                    CalcularDistancia(nodoActual, vecino);

                if (nuevoCosteInicio < vecino.costeInicio || !abiertos.Contains(vecino))
                {
                    vecino.costeInicio = nuevoCosteInicio;
                    vecino.costeObjetivo = CalcularDistancia(vecino, objetivo);
                    vecino.padre = nodoActual;

                    if (!abiertos.Contains(vecino))
                    {
                        abiertos.Add(vecino);
                    }
                }
            }
        }

        return new List<Nodo>();
    }

    List<Nodo> CrearCamino(Nodo inicio, Nodo objetivo)
    {
        List<Nodo> camino = new List<Nodo>();

        Nodo nodoActual = objetivo;

        while (nodoActual != inicio)
        {
            camino.Add(nodoActual);
            nodoActual = nodoActual.padre;
        }

        camino.Reverse();

        return camino;
    }

    // public List<Nodo> ObtenerCamino()
    // {
    //     return camino;
    // }
}
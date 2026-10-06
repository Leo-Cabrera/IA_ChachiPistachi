using UnityEngine;

public class Nodo
{
    public bool caminable;
    public Vector3 posicionMundo;

    public int posX;
    public int posY;

    public int costeInicio;
    public int costeObjetivo;

    public Nodo padre;

    public int CosteTotal
    {
        get
        {
            return costeInicio + costeObjetivo;
        }
    }

    public Nodo(bool caminable, Vector3 posicionMundo, int posX, int posY)
    {
        this.caminable = caminable;
        this.posicionMundo = posicionMundo;
        this.posX = posX;
        this.posY = posY;
        this.costeInicio = 0;
        this.costeObjetivo = 0;
        this.padre = null;
    }
}
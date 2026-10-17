
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcController : MonoBehaviour
{
    //esta clase es también una plantilla para los controladores de los npc

    //datos comunes a todos los npc
    private Parilla parilla;
    private List<Nodo> camino;
    private int indiceCamino;
    private Nodo ultimoNodoObjetivo;

    public Transform target;
    
    public Vector3 velJugador;
    public float TiempoCambio = 4f;
    public float crono;
    public Vector3 direccionRandom;

    private SphereCollider miSphereCollider;


    Vector3 targetPosition;
    Vector3 delta;
    Vector3 steering;
    Vector3 currentVelocity;


    public StateMachine FSM { get; private set; }

    private void Start()
    {
        // Crear la máquina de estados
        FSM = new StateMachine();

        // Crear y registrar todos los estados comunes
        ConfigureStates();

        
    }

    private void Update()
    {
       //Solo hay que llamar a la máquina de estado porque ella dirigirá
       // a las funciones que toquen
        FSM?.Update();
    }
    protected abstract void ConfigureStates();


    //Métodos comunes a todos los NPC


}














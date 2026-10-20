
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcController : MonoBehaviour
{
    //esta clase es tambi�n una plantilla para los controladores de los npc

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

    public List<Nodo> Camino => camino;


    Vector3 targetPosition;
    Vector3 delta;
    Vector3 steering;
    Vector3 currentVelocity;


    public StateMachine FSM { get; private set; }

    private void Awake()  //Awake se ejecuta antes que Start, as� la m�quina de estados se crea antes de que un npc pueda detectar algo
    {
        // Crear la m�quina de estados
        FSM = new StateMachine();

        // Crear y registrar todos los estados comunes
        ConfigureStates();

        //codigo com�n
        parilla = FindFirstObjectByType<Parilla>();

        miSphereCollider =
            (SphereCollider)GetComponent(typeof(SphereCollider));

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }

        EligirNuevaDireccion();


    }

    private void Update()
    {
       //Solo hay que llamar a la m�quina de estado porque ella dirigir�
       // a las funciones que toquen
        FSM?.Update();
    }
    protected abstract void ConfigureStates();


    //M�todos comunes a todos los NPC
    public void EligirNuevaDireccion()
    {
        float grado =
            Random.Range(0f, 360f);

        direccionRandom =
            Quaternion.Euler(0, grado, 0) *
            Vector3.forward;
    }

    //lo llamaremos tanto cuando el npc est� en patrol o chase, tendr� que cambiar su ruta ya sea persiguiendo o dejando de perseguir
    public void ReiniciarCamino()
    {
        camino = null;  //para desechar la ruta que ya tenia guardada
        indiceCamino = 0;  //para poder tener una futura ruta 
        ultimoNodoObjetivo = null;  //para olvidar el destino anterior 
    }

    //los 3 enemigos usar�n esto, sus estados decidir�n cu�ndo 
    public void ActualizarCamino(Vector3 posicionObjetivo)  //pasamos un punto en el mundo 3d
    {
        if (parilla == null) //necesitamos el mundo dividido en nodos sino salimos del metodo para evitar errores
            return;

        Nodo nodoNpc = parilla.ObtenerNodoDesdePosicion(transform.position);  //guardamos el nodo en el que se encuentra npc

        Nodo nodoObjetivo = parilla.ObtenerNodoDesdePosicion(posicionObjetivo);  //guardamos el nodo donde est� el jugador

        //sirve para ver si necesitamos calcular una nueva ruta y reemplazar la guardada, si ocurre algo entonces entra y se encarga
        if (camino == null || camino.Count == 0 || indiceCamino >= camino.Count || nodoObjetivo != ultimoNodoObjetivo)
        //acaba de entrar en chase y no hay ruta todav�a, hemos llegado al final de la ruta, la lista tiene menos nodos
        //de los necesitamos (hemos terminado la ruta), si el jugador se mueve a otra casilla de la que teniamos calcula ha cambiado ruta
        {
            camino = parilla.BuscarCamino(nodoNpc, nodoObjetivo);  //usamos buscar para obtener una lista de nodos
            indiceCamino = 0;  //empezamos por el primer nodo
            ultimoNodoObjetivo = nodoObjetivo;  //nodo de la posici�n recibida, que puede ser la del jugador o una posici�n predicha
        }
    }

    public void MoverPorCamino(float velocidad, float rotacionVel) //parametros que usaremos para avanzar y girar
    {
        if (camino == null || camino.Count == 0)  //por si la ruta est� vac�a o no existe
            return;

        if (indiceCamino >= camino.Count)  //evita acceder a otro nodo si se ha terminado la ruta (se tendr� que calcular una nueva)
            return;

        Vector3 destino = camino[indiceCamino].posicionMundo;  //selecciona al nodo que toca ir 
        Vector3 direccion = destino - transform.position;  //calcula el vector desde el enemigo hasta el nodo de destino (donde se mueve)
        direccion.y = 0f;  //porque los enemigos se mueven por el suelo 

        if (direccion.magnitude < 0.1f) //la distancia al nodo, si es menos 0.1 es porque lo hemos alcanzado
        {
            indiceCamino++;  //sumamos el indice para dirigirnos al siguiente nodo
            return;
        }

        //calcula c�mo debe girarse el enemigo para mirar al nodo actual de la ruta
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);

        //igual que en patrol, es para que gire gradualmente en fotogramas desde su posici�n hacia la nueva de donde mira
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);

        //aqui no avanza con transform.forward como en patrulla porque queremos que vaya hacia un nodo (direccion.normalized)
        transform.position += direccion.normalized * velocidad * Time.deltaTime;
    }

    public bool EstaEnNodoDe(Vector3 posicionObjetivo) //nos va a servir para que el fantasma vaya hasta el nodo del grito
    {
        if (parilla == null) return false;  //para comprobar que tenemos el mapa con nodos

        Nodo nodoActual = parilla.ObtenerNodoDesdePosicion(transform.position);  //guarda nodo donde esta el fantasma
        Nodo nodoObjetivo = parilla.ObtenerNodoDesdePosicion(posicionObjetivo);  //guarda el nodo del objetivo (la armadura)

        return nodoActual == nodoObjetivo; //cuando esto devuelva true ser� que ha llegado hasta el nodo objetivo
    }

}














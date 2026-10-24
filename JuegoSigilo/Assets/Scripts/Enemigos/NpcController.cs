
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcController : MonoBehaviour
{
    //esta clase es tambien una plantilla para los controladores de los npc

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


    [Header("Obstacle Avoidance")]
    [SerializeField] private float distanciaDeteccion = 2f;
    [SerializeField] private float radioEvasion = 0.4f;
    [SerializeField] private float fuerzaEvasion = 2f;
    [SerializeField] private LayerMask capaObstaculos;
    [SerializeField] private LayerMask capaEnemigos;


    public StateMachine FSM { get; private set; }

    private void Awake()  //Awake se ejecuta antes que Start, asi la maquina de estados se crea antes de que un npc pueda detectar algo
    {
        // Crear la maquina de estados
        FSM = new StateMachine();

        // Crear y registrar todos los estados comunes
        ConfigureStates();

        //codigo comun
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
       //Solo hay que llamar a la maquina de estado porque ella dirigir
       // a las funciones que toquen
        FSM?.Update();
    }
    protected abstract void ConfigureStates();


    //Metodos comunes a todos los NPC
    public void EligirNuevaDireccion()
    {
        float grado =
            Random.Range(0f, 360f);

        direccionRandom =
            Quaternion.Euler(0, grado, 0) *
            Vector3.forward;
    }

    //lo llamaremos tanto cuando el npc esta en patrol o chase, tendra que cambiar su ruta ya sea persiguiendo o dejando de perseguir
    public void ReiniciarCamino()
    {
        camino = null;  //para desechar la ruta que ya tenia guardada
        indiceCamino = 0;  //para poder tener una futura ruta 
        ultimoNodoObjetivo = null;  //para olvidar el destino anterior 
    }

    //los 3 enemigos usaran esto, sus estados decidiran cuando 
    public void ActualizarCamino(Vector3 posicionObjetivo)  //pasamos un punto en el mundo 3d
    {
        if (parilla == null) //necesitamos el mundo dividido en nodos sino salimos del metodo para evitar errores
            return;

        Nodo nodoNpc = parilla.ObtenerNodoDesdePosicion(transform.position);  //guardamos el nodo en el que se encuentra npc

        Nodo nodoObjetivo = parilla.ObtenerNodoDesdePosicion(posicionObjetivo);  //guardamos el nodo donde esta el jugador

        //sirve para ver si necesitamos calcular una nueva ruta y reemplazar la guardada, si ocurre algo entonces entra y se encarga
        if (camino == null || camino.Count == 0 || indiceCamino >= camino.Count || nodoObjetivo != ultimoNodoObjetivo)
        //acaba de entrar en chase y no hay ruta todavia, hemos llegado al final de la ruta, la lista tiene menos nodos
        //de los necesitamos (hemos terminado la ruta), si el jugador se mueve a otra casilla de la que teniamos calcula ha cambiado ruta
        {
            camino = parilla.BuscarCamino(nodoNpc, nodoObjetivo);  //usamos buscar para obtener una lista de nodos
            indiceCamino = 0;  //empezamos por el primer nodo
            ultimoNodoObjetivo = nodoObjetivo;  //nodo de la posicion recibida, que puede ser la del jugador o una posicion predicha
        }
    }

    public void MoverPorCamino(float velocidad, float rotacionVel) //parametros que usaremos para avanzar y girar
    {
        if (camino == null || camino.Count == 0)  //por si la ruta esta vacia o no existe
            return;

        if (indiceCamino >= camino.Count)  //evita acceder a otro nodo si se ha terminado la ruta (se tendra que calcular una nueva)
            return;

        Vector3 destino = camino[indiceCamino].posicionMundo;  //selecciona al nodo que toca ir 
        Vector3 direccion = destino - transform.position;  //calcula el vector desde el enemigo hasta el nodo de destino (donde se mueve)
        direccion.y = 0f;  //porque los enemigos se mueven por el suelo 

        if (direccion.magnitude < 0.1f) //la distancia al nodo, si es menos 0.1 es porque lo hemos alcanzado
        {
            indiceCamino++;  //sumamos el indice para dirigirnos al siguiente nodo
            return;
        }

        //calcula como debe girarse el enemigo para mirar al nodo actual de la ruta
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);

        //igual que en patrol, es para que gire gradualmente en fotogramas desde su posicion hacia la nueva de donde mira
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);

        //aqui no avanza con transform.forward como en patrulla porque queremos que vaya hacia un nodo (direccion.normalized)
        transform.position += direccion.normalized * velocidad * Time.deltaTime;
    }

    public bool EstaEnNodoDe(Vector3 posicionObjetivo) //nos va a servir para que el fantasma vaya hasta el nodo del grito
    {
        if (parilla == null) return false;  //para comprobar que tenemos el mapa con nodos

        Nodo nodoActual = parilla.ObtenerNodoDesdePosicion(transform.position);  //guarda nodo donde esta el fantasma
        Nodo nodoObjetivo = parilla.ObtenerNodoDesdePosicion(posicionObjetivo);  //guarda el nodo del objetivo (la armadura)

        return nodoActual == nodoObjetivo; //cuando esto devuelva true sera que ha llegado hasta el nodo objetivo
    }


    public void MoverPatrullando(float velocidad, float rotacionVel)
    {
        // Guarda la dirección aleatoria que ha elegido el NPC para patrullar
        Vector3 direccion = direccionRandom;
        // Eliminamos el movimiento vertical para que se mueva por el suelo
        direccion.y = 0f;

          // Si la dirección es prácticamente cero, no hacemos nada y salimos del método
        if (direccion.sqrMagnitude < 0.001f)
            return;

        // Pongo el origen de la detección medio metro por encima del suelo
        Vector3 origen = transform.position + Vector3.up * 0.5f;

        // Comprobamos si hay un obstáculo delante del NPC
        // SphereCast lanza una esfera imaginaria para detectar paredes o objetos
        // 0.4f es el radio de la esfera y 1.5f la distancia de detección
        // capaObstaculos indica qué capas pueden considerarse obstáculos
        if (Physics.SphereCast(origen, 0.4f, direccion.normalized, out RaycastHit impacto, 1.5f, capaObstaculos, QueryTriggerInteraction.Ignore))
        {
            // Si encuentra un obstáculo, utilizamos su normal para calcular una nueva dirección que permita cambiar el rumbo del NPC ProjectOnPlane elimina la componente vertical de esa dirección
            direccionRandom = Vector3.ProjectOnPlane(impacto.normal, Vector3.up).normalized;

            // Detenemos el movimiento de este fotograma para evitar avanzar directamente hacia el obstáculo.
            return;
        }


        // Calculamos la rotación deseada para que el NPC mire hacia la dirección aleatoria
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        //Giramos progresivamente desde la orientación actual hasta la que quermos, usando Slerp para suavizar el giro
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);
        //Avanzamos hacia delante según la orientación del NPC
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

}














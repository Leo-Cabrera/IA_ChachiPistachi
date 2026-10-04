using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public Transform target;
    public float velocidad = 3f;
    public float RotacionVel = 5f;

    public float tiempoPrediccion = 3f;
    public Vector3 velJugador;

    public float TiempoCambio = 4f;
    public float crono;
    public Vector3 direccionRandom;

    private SphereCollider miSphereCollider;

    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = .4f;

    Vector3 targetPosition;
    Vector3 delta;
    Vector3 steering;
    Vector3 currentVelocity;

    // private bool dentroAhora = false;
    // private bool dentroAntes = false;

    int roleAssigned = -1; // -1: no asignado, 0: SEEK, 1: PURSUIT

    enum EnemyState
    {
        SEEK,
        PURSUIT,
        PATROL
    }

    EnemyState currentState;

    void Start()
    {
        currentState = EnemyState.PATROL;
        miSphereCollider = (SphereCollider)GetComponent(typeof(SphereCollider));

        if (target == null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
                target = player.transform;
        }
        EligirNuevaDireccion();
    }

    void Update() {
        Debug.Log("Current state: " + currentState);
        
        if (target != null)
        {
            //float distanceToTarget = Vector3.Distance(transform.position, target.position);
            //float radioSphereCollider = ObtenerRadioSphereCollider();

            if(currentState == EnemyState.PATROL) {
                Patrullar();
            }
            else if (currentState == EnemyState.SEEK || currentState == EnemyState.PURSUIT) {
                Atacar();
            }
        }
    }

    float ObtenerRadioSphereCollider()
    {
        if (miSphereCollider != null)
        {
            float escalaMaxima = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            return miSphereCollider.radius * escalaMaxima;
        }

        return 7f; // Radio por defecto si no se encuentra el SphereCollider
    }

    void OnTriggerEnter(Collider sphereCollider)
    {
        if (sphereCollider.gameObject.CompareTag("Player") && sphereCollider is CapsuleCollider)
        {
            roleAssigned = Random.Range(0, 2); // Genera un número aleatorio entre 0 y 1
            //Debug.Log("Role assigned: " + roleAssigned);
            if (roleAssigned == 0) {
                currentState = EnemyState.SEEK;
            }
            else {
                currentState = EnemyState.PURSUIT;
            }
        }
        else {
            return;
        }
    }

    void Atacar() {
        if (currentState == EnemyState.SEEK)  //SEEK
        {
            //SEEK BEHAVIOUR: Un poco chustera pero bueno va que es lo importante aquí
            targetPosition = target.position;

            delta = targetPosition - transform.position;
            delta.y = 0; // Ignorar la componente vertical para el movimiento en el plano horizontal
            Vector3 desiredVelocity = delta.normalized * maxSpeed;

            steering = desiredVelocity - currentVelocity;
            currentVelocity = currentVelocity + steering * Time.deltaTime;
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

            float brakingFactor = Mathf.Sqrt(Mathf.Clamp01(delta.magnitude / arriveDistance));
            currentVelocity = currentVelocity * brakingFactor;

            Quaternion rotacionDeseada = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);

            transform.position = transform.position + (Vector3)currentVelocity * Time.deltaTime;
        }
        else if (currentState == EnemyState.PURSUIT)   //PURSUIT
        {
            velJugador = ObtenerVelocidadJugador();

            Vector3 posPredicha = target.position + velJugador * tiempoPrediccion;

            posPredicha.y = transform.position.y;

            Vector3 direccion = posPredicha - transform.position;

            Vector3 posDeseada = direccion.normalized * maxSpeed;

            steering = posDeseada - currentVelocity;

            currentVelocity += steering * Time.deltaTime;
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);

            transform.position = transform.position + (Vector3)currentVelocity * Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider sphereCollider)
    {
        if (sphereCollider.gameObject.CompareTag("Player") && sphereCollider is CapsuleCollider)
        {
            currentState = EnemyState.PATROL;
        }
    }

    void Patrullar()
    {
        crono += Time.deltaTime;
        if (crono >= TiempoCambio)
        {
            EligirNuevaDireccion();
            crono = 0;
        }

        Quaternion rotacionDeseada = Quaternion.LookRotation(direccionRandom);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);

        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    void EligirNuevaDireccion()
    {
        float grado = Random.Range(0f, 360f);
        direccionRandom = Quaternion.Euler(0, grado, 0) * Vector3.forward;
    }


    Vector3 ObtenerVelocidadJugador()
    {
        Rigidbody rbJugador = target.GetComponent<Rigidbody>();

        if (rbJugador != null)
        {
            return rbJugador.linearVelocity;
        }
        return Vector3.zero;
    }
}
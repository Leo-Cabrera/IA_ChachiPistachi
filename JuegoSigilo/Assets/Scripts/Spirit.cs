using UnityEngine;
using System.Collections;

public class Spirit : MonoBehaviour
{
    public Transform target;
    public float velocidad = 5f;
    public float RotacionVel = 5f;

    public float tiempoPrediccion = 3f;
    public Vector3 velJugador;

    public float TiempoCambio = 4f;
    public float crono;
    public Vector3 direccionRandom;

    private SphereCollider miSphereCollider;

    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float arriveDistance = .4f;

    Vector3 targetPosition;
    Vector3 delta;
    Vector3 steering;
    Vector3 currentVelocity;

    // private bool dentroAhora = false;
    // private bool dentroAntes = false;

    enum EnemyState
    {
        PATROL,
        SEEK
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
            else if (currentState == EnemyState.SEEK) {
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

    public void OnAlerted() {
        currentState = EnemyState.SEEK;
    }

    void Atacar() {
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
}
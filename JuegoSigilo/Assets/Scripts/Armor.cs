using UnityEngine;
using System.Collections;

public class Armor : MonoBehaviour
{
    public Transform target;
    public float velocidad = 1f;
    public float RotacionVel = 5f;

    public float tiempoPrediccion = 3f;
    public Vector3 velJugador;

    public float TiempoCambio = 4f;
    public float crono;
    public Vector3 direccionRandom;

    private SphereCollider miSphereCollider;

    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float arriveDistance = .4f;

    Vector3 targetPosition;
    Vector3 delta;
    Vector3 steering;
    Vector3 currentVelocity;

    float radioDeteccion = 10f; // Radio de detección para alertar a los espíritus

    // private bool dentroAhora = false;
    // private bool dentroAntes = false;

    bool beenAlerted = false;

    public AudioSource alertSoundSource; // AudioSource para reproducir el sonido de alerta

    enum EnemyState
    {
        SEEK,
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

    void OnTriggerEnter(Collider sphereCollider)
    {
        if (sphereCollider.gameObject.CompareTag("Player") && sphereCollider is CapsuleCollider)
        {
            currentState = EnemyState.SEEK;
            alertSoundSource.Play(); // Reproducir el sonido de alerta al entrar en contacto con el jugador
            if (!beenAlerted) {
                alertSpirits();
                beenAlerted = true;
            }
        }
        else {
            return;
        }
    }

    void alertSpirits() {
        Spirit[] potentialSpirits = FindObjectsOfType<Spirit>();
        Spirit[] spiritsAlerted = new Spirit[potentialSpirits.Length];
        int alertCount = 0;
        Transform spiritTransform;

        foreach (Spirit spirit in potentialSpirits) {
            spiritTransform = spirit.GetComponent<Transform>();
            float distancia = Vector3.Distance(transform.position, spiritTransform.position);
            if (distancia <= radioDeteccion && spirit != null) {
                spiritsAlerted[alertCount] = spirit;
                alertCount++;
            }
        }

        foreach (Spirit spirit in spiritsAlerted) {
            if (spirit != null) {
                spirit.OnAlerted();
            }
        }
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
            beenAlerted = false; // Reset the alert status when the player exits the trigger
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
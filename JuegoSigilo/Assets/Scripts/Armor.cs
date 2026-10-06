using UnityEngine;
using System.Collections;

public class Armor : MonoBehaviour
{
    private Parilla parilla;
    private List<Nodo> camino;
    private int indiceCamino;
    private Nodo ultimoNodoObjetivo;

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

    float radioDeteccion = 10f;

    bool beenAlerted = false;

    public AudioSource alertSoundSource;

    enum EnemyState
    {
        SEEK,
        PATROL
    }

    EnemyState currentState;

    void Start()
    {
        parilla = FindObjectOfType<Parilla>();

        currentState = EnemyState.PATROL;

        miSphereCollider =
            (SphereCollider)GetComponent(typeof(SphereCollider));

        if (target == null)
        {
            GameObject player =
                GameObject.Find("Player");

            if (player != null)
                target = player.transform;
        }
        EligirNuevaDireccion();
    }

    void Update()
    {
        if (target != null)
        {
            if (currentState == EnemyState.PATROL)
            {
                Patrullar();
            }
            else if (currentState == EnemyState.SEEK)
            {
                Atacar();
            }
        }
    }

    float ObtenerRadioSphereCollider()
    {
        if (miSphereCollider != null)
        {
            float escalaMaxima =
                Mathf.Max(
                    transform.lossyScale.x,
                    transform.lossyScale.y,
                    transform.lossyScale.z
                );

            return miSphereCollider.radius *
                   escalaMaxima;
        }

        return 7f;
    }

    void alertSpirits()
    {
        Spirit[] potentialSpirits =
            FindObjectsOfType<Spirit>();

        Spirit[] spiritsAlerted =
            new Spirit[potentialSpirits.Length];

        int alertCount = 0;

        Transform spiritTransform;

        foreach (Spirit spirit in potentialSpirits)
        {
            spiritTransform =
                spirit.GetComponent<Transform>();

            float distancia =
                Vector3.Distance(
                    transform.position,
                    spiritTransform.position
                );

            if (distancia <= radioDeteccion &&
                spirit != null)
            {
                spiritsAlerted[alertCount] =
                    spirit;

                alertCount++;
            }
        }

        foreach (Spirit spirit in spiritsAlerted)
        {
            if (spirit != null)
            {
                spirit.OnAlerted();
            }
        }
    }

    void Atacar()
    {
        if (parilla == null)
            return;

        Nodo nodoArmor =
            parilla.ObtenerNodoDesdePosicion(
                transform.position
            );

        Nodo nodoObjetivo =
            parilla.ObtenerNodoDesdePosicion(
                target.position
            );

        if (camino == null ||
            camino.Count == 0 ||
            indiceCamino >= camino.Count ||
            nodoObjetivo != ultimoNodoObjetivo)
        {
            camino =
                parilla.BuscarCamino(
                    nodoArmor,
                    nodoObjetivo
                );

            indiceCamino = 0;
            ultimoNodoObjetivo = nodoObjetivo;
        }

        steering = desiredVelocity - currentVelocity;
        currentVelocity = currentVelocity + steering * Time.deltaTime;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        float brakingFactor = Mathf.Sqrt(Mathf.Clamp01(delta.magnitude / arriveDistance));
        currentVelocity = currentVelocity * brakingFactor;

        Quaternion rotacionDeseada = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);

        Vector3 destino =
            camino[indiceCamino].posicionMundo;

        Vector3 direccion =
            destino - transform.position;

        direccion.y = 0;

        if (direccion.magnitude < 0.1f)
        {
            indiceCamino++;
            return;
        }

        Quaternion rotacionDeseada =
            Quaternion.LookRotation(direccion);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionDeseada,
                RotacionVel * Time.deltaTime
            );

        transform.position +=
            direccion.normalized *
            velocidad *
            Time.deltaTime;
    }

    void OnTriggerEnter(Collider sphereCollider)
    {
        if (sphereCollider.gameObject.CompareTag("Player") &&
            sphereCollider is CapsuleCollider)
        {
            currentState = EnemyState.SEEK;

            camino = null;
            indiceCamino = 0;
            ultimoNodoObjetivo = null;

            alertSoundSource.Play();

            if (!beenAlerted)
            {
                alertSpirits();
                beenAlerted = true;
            }
        }
    }

    void OnTriggerExit(Collider sphereCollider)
    {
        if (sphereCollider.gameObject.CompareTag("Player") &&
            sphereCollider is CapsuleCollider)
        {
            currentState = EnemyState.PATROL;

            camino = null;
            indiceCamino = 0;
            ultimoNodoObjetivo = null;

            beenAlerted = false;

            crono = 0f;

            EligirNuevaDireccion();
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

        Quaternion rotacionDeseada =
            Quaternion.LookRotation(direccionRandom);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionDeseada,
                RotacionVel * Time.deltaTime
            );

        transform.position +=
            transform.forward *
            velocidad *
            Time.deltaTime;
    }

    void EligirNuevaDireccion()
    {
        float grado =
            Random.Range(0f, 360f);

        direccionRandom =
            Quaternion.Euler(0, grado, 0) *
            Vector3.forward;
    }

    void OnDrawGizmos()
    {
        if (camino == null)
            return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < camino.Count; i++)
        {
            Vector3 posicion =
                camino[i].posicionMundo +
                Vector3.up * 0.25f;

            Gizmos.DrawCube(
                posicion,
                Vector3.one * 0.9f
            );

            if (i < camino.Count - 1)
            {
                Vector3 siguiente =
                    camino[i + 1].posicionMundo +
                    Vector3.up * 0.25f;

                Gizmos.DrawLine(
                    posicion,
                    siguiente
                );
            }
        }
    }
}
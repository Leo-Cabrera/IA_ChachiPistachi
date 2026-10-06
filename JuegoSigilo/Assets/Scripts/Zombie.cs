using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private Parilla parilla;

    private List<Nodo> camino;
    private int indiceCamino;
    private Nodo ultimoNodoObjetivo;
    private Nodo ultimoNodoObjetivo;

    int roleAssigned = -1;

    enum EnemyState
    {
        SEEK,
        PURSUIT,
        PATROL
        PATROL
    }

    EnemyState currentState;


    void Start()
    {
        parilla = FindObjectOfType<Parilla>();

        currentState = EnemyState.PATROL;

        miSphereCollider =
            (SphereCollider)GetComponent(typeof(SphereCollider));

        miSphereCollider =
            (SphereCollider)GetComponent(typeof(SphereCollider));

        if (target == null)
        {
            GameObject player = GameObject.Find("Player");


            if (player != null)
                target = player.transform;
        }


        EligirNuevaDireccion();
    }

    void Update()
    {
        if (target == null)
            return;

        if (currentState == EnemyState.PATROL)
        {
            Patrullar();
        }
        else if (currentState == EnemyState.SEEK)
        {
            SeguirCamino(false);
        }
        else if (currentState == EnemyState.PURSUIT)
        if (currentState == EnemyState.PATROL)
        {
            Patrullar();
        }
        else if (currentState == EnemyState.SEEK)
        {
            SeguirCamino(false);
        }
        else if (currentState == EnemyState.PURSUIT)
        {
            SeguirCamino(true);
            SeguirCamino(true);
        }
    }

    void SeguirCamino(bool pursuit)
    {
        if (parilla == null) return;

        Vector3 posicionObjetivo;

        if (pursuit)
        {
            velJugador = ObtenerVelocidadJugador();

            Vector3 posPredicha =
                target.position + velJugador * tiempoPrediccion;

            posPredicha.y = transform.position.y;

            posicionObjetivo = posPredicha;
        }
        else
        {
            posicionObjetivo = target.position;
        }

        posicionObjetivo.y = transform.position.y;

        Nodo nodoZombie =
            parilla.ObtenerNodoDesdePosicion(transform.position);

        Nodo nodoObjetivo =
            parilla.ObtenerNodoDesdePosicion(posicionObjetivo);

        if (camino == null ||
            camino.Count == 0 ||
            indiceCamino >= camino.Count ||
            nodoObjetivo != ultimoNodoObjetivo)
        {
            camino =
                parilla.BuscarCamino(
                    nodoZombie,
                    nodoObjetivo
                );

            indiceCamino = 0;
            ultimoNodoObjetivo = nodoObjetivo;
        }

        MoverPorCamino();
    }

    void MoverPorCamino()
    {
        if (camino == null || camino.Count == 0)
            return;

        if (indiceCamino >= camino.Count)
            return;

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

        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);

        transform.rotation = Quaternion.Slerp( transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);

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
            roleAssigned = Random.Range(0, 2);

            if (roleAssigned == 0)
            {
                currentState = EnemyState.SEEK;
            }
            else
            {
            else
            {
                currentState = EnemyState.PURSUIT;
            }

            camino = null;
            indiceCamino = 0;
            ultimoNodoObjetivo = null;

            Debug.Log(
                gameObject.name +
                " -> " +
                currentState
            );
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

            velJugador = Vector3.zero;
            currentVelocity = Vector3.zero;

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

    Vector3 ObtenerVelocidadJugador()
    {
        Rigidbody rbJugador =
            target.GetComponent<Rigidbody>();

        if (rbJugador != null)
        {
            return rbJugador.linearVelocity;
        }


        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(
            transform.position + Vector3.up * 2f,
            0.5f
        );

        if (camino == null)
            return;

        Gizmos.color = Color.cyan;

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
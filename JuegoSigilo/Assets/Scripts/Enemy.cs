using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float velocidad = 3f;
    public float RotacionVel = 5f;

    public float TiempoCambio = 4f;
    public float crono;
    public Vector3 direccionRandom;

    private SphereCollider miSphereCollider;

    void Start()
    {
        // Sintaxis alternativa que no da error CS0411:
        miSphereCollider = (SphereCollider)GetComponent(typeof(SphereCollider));

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
        if (target == null) return;

        float radioDeteccion = ObtenerRadioSphereCollider();

        float distancia = Vector3.Distance(transform.position, target.position);

        if (distancia <= radioDeteccion)
        {
            SeguirObjetivo(target.position);
        }
        else
        {
            Patrullar();
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

    void SeguirObjetivo(Vector3 posicionObjetivo)
    {
        Vector3 direccion = (posicionObjetivo - transform.position);
        direccion.y = 0;

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, RotacionVel * Time.deltaTime);
            transform.position += transform.forward * velocidad * Time.deltaTime;
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
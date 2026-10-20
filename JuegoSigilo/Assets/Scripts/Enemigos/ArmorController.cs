
using UnityEngine;

public class ArmorController : NpcController
{
    public float velocidad = 1f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float arriveDistance = .4f;
    [SerializeField] private float radioDeteccion = 10f;
    public AudioSource alertSoundSource; //sonido del grito

    // NUEVO: aplicar el volumen de efectos guardado
    private void Start()
    {
        if (alertSoundSource != null)
        {
            alertSoundSource.volume =
                PlayerPrefs.GetFloat("SFXVolume", 0.3f);
        }
    }

    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)

        //creamos el estado de patrulla de la armadura
        PatrolState patrullaArmor = new PatrolState(this, velocidad, RotacionVel);

        //guardamos la regla que este enemigo sigue, si detecta al jugador pasa a chase
        patrullaArmor.ConfigurarTransicion("PlayerDetected", typeof(ChaseState));

        //registramos la patrulla que acabamos de configurar para que pueda tener esa regla
        FSM.RegisterState(patrullaArmor);

        //registramos ahora el estado de persecucion
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        //comienza con el estado de patrulla
        FSM.SetState(typeof(PatrolState));
    }

    //se llama si algo entra en la zona de trigger de la armadura
    private void OnTriggerEnter(Collider otroCollider)
    {
        if (otroCollider.gameObject.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerDetected");

            if (alertSoundSource != null)
            {
                // NUEVO: actualizar el volumen antes de reproducir
                alertSoundSource.volume =
                    PlayerPrefs.GetFloat("SFXVolume", 0.3f);

                alertSoundSource.Play();
            }

            AlertarFantasmasCercanos();
        }
    }

    //se llama si algo sale de la zona de trigger de la armadura
    private void OnTriggerExit(Collider otroCollider)
    {
        if (otroCollider.gameObject.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerLost");
        }
    }

    private void AlertarFantasmasCercanos()
    {
        //almacena los fantasmas activos
        SpiritController[] fantasmas = FindObjectsByType<SpiritController>(FindObjectsSortMode.None);

        foreach (SpiritController fantasma in fantasmas)
        {
            //mira la distancia de la armadura al fantasma
            float distancia = Vector3.Distance(transform.position, fantasma.transform.position);

            if (distancia <= radioDeteccion)
            {
                fantasma.OnAlerted(transform.position);
            }
        }
    }

    
    private void OnDrawGizmos()
    {
        if (Camino == null || Camino.Count == 0)
            return;

        Gizmos.color = Color.orange;

        foreach (Nodo nodo in Camino)
        {
            Gizmos.DrawCube(
                nodo.posicionMundo,
                new Vector3(0.8f, 0.8f, 0.8f)
            );
        }
    }

}

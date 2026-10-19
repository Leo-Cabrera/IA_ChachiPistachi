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

    //se llama si algo entra en la zona de trigger de la armadura, otroCollider será lo que haya entrado
    private void OnTriggerEnter(Collider otroCollider)
    {
        if (otroCollider.gameObject.CompareTag("Player") && otroCollider is CapsuleCollider) //si es el jugador el que triggerea
        {
            FSM.TriggerEvent("PlayerDetected"); //ha ocurrido el evento que hace que armor pase de patrol a chase

            if (alertSoundSource != null) //si tenemos un sonido puesto, para evitar errores por si no hubiera
            {
                alertSoundSource.Play(); //entonces que se reproduzca
            }

            AlertarFantasmasCercanos(); //llamamos al metodo que avisa a los fantasmas
        }
    }

    //se llama si algo sale de la zona de trigger de la armadura, otroCollider será lo que haya salido
    private void OnTriggerExit(Collider otroCollider)
    {
        if (otroCollider.gameObject.CompareTag("Player") && otroCollider is CapsuleCollider)  //el jugador ha salido de la zona trigger
        {
            FSM.TriggerEvent("PlayerLost");  //el evento ahora es que ya no ve al jugador, la maquina de estados hará transicion a patrol
        }
    }

    private void AlertarFantasmasCercanos()
    {
        //almacena los fantasmas activos
        SpiritController[] fantasmas = FindObjectsByType<SpiritController>(FindObjectsSortMode.None);

        foreach (SpiritController fantasma in fantasmas) //va seleccionando los fantasmas que tenemos almacenados
        {
            //mira la distancia de la armadura al fantasma 
            float distancia = Vector3.Distance(transform.position, fantasma.transform.position); 

            if (distancia <= radioDeteccion) //si el fantasma se encuentra dentro del radio que cubre la armadura
            {
                fantasma.OnAlerted(transform.position); //envia a los fantasmas a la posicion del grito de la armadura
            }
        }
    }
}

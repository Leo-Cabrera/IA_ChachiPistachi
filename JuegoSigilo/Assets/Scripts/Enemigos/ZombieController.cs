using UnityEngine;

public class ZombieController : NpcController
{
    public float velocidad = 3f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 0.5f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = .4f;
    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)

        //guardamos el estado de patrulla para luego poder tener en cuentas sus dos reglas antes de registrarlo a la fsm
        PatrolState patrullaZombie = new PatrolState(this, velocidad, RotacionVel);
        patrullaZombie.ConfigurarTransicion("SeekChosen", typeof(ChaseState));  //añadimos el comportamiento del estado
        patrullaZombie.ConfigurarTransicion("PursuitChosen", typeof(ZombiePursuitState));
        FSM.RegisterState(patrullaZombie);  //ya la registramos teniendo sus posibles transiciones

        //registramos ahora el estado de persecucion
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        //registramos el estado de pursuit
        FSM.RegisterState(new ZombiePursuitState(this));

        //comienza con el estado de patrulla
        FSM.SetState(typeof(PatrolState));

    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        //que el zombie patrulle y que quien haya entrado sea la capsula del jugador
        if (FSM.CurrentState is PatrolState && otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            int eleccion = Random.Range(0, 2);  //para obtener un 0 o un 1

            if (eleccion == 0)
                FSM.TriggerEvent("SeekChosen");  //si es 0 entonces seek, hará chase
            else
                FSM.TriggerEvent("PursuitChosen");  //si es 1 entonces pursuit

            Debug.Log(gameObject.name + " -> " + FSM.CurrentState.GetType().Name);  //para ver en la consola qué ha tocado
        }
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        //si la capsula del jugador salen del rango del zombie
        if (otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerLost");  //cambiamos de etiqueta que hará que se pueda cambiar de estado
        }
    }
}

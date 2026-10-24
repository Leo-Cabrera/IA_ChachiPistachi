using UnityEngine;

public class ZombieController : NpcController
{
    public float velocidad = 3f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 0.5f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = .4f;

    [Header("Seguimiento del jefe")]
    public Transform jefe;

    protected override void ConfigureStates()
    {
        // Estado inicial: seguir al jefe
        FSM.RegisterState(new LeaderFollowingState(this));

        // Estado de persecución normal
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        // Estado de persecución predictiva
        FSM.RegisterState(new ZombiePursuitState(this));

        // Estado de patrulla
        FSM.RegisterState(new PatrolState(this, velocidad, RotacionVel));

        // Empezamos siguiendo al jefe
        FSM.SetState(typeof(LeaderFollowingState));

    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        //Si entra el jugador mientras seguimos al jefe
        if (FSM.CurrentState is LeaderFollowingState  && otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            int eleccion = Random.Range(0, 2);  //para obtener un 0 o un 1

            if (eleccion == 0)
                FSM.TriggerEvent("SeekChosen");  //si es 0 entonces seek, hara chase
            else
                FSM.TriggerEvent("PursuitChosen");  //si es 1 entonces pursuit

            Debug.Log(gameObject.name + " -> " + FSM.CurrentState.GetType().Name);  //para ver en la consola que ha tocado
        }

        //Si es el jefe mientras patrulla, pasamos a seguirlo
        if (FSM.CurrentState is PatrolState && otroCollider.CompareTag("Boss"))
        {
            FSM.TriggerEvent("FollowBoss");
            Debug.Log(gameObject.name + ": vuelve a seguir al jefe.");
        }
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        //si la capsula del jugador salen del rango del zombie
        if (otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider && (FSM.CurrentState is ChaseState || FSM.CurrentState is ZombiePursuitState))
        {
            FSM.TriggerEvent("PlayerLost");  //cambiamos de etiqueta que hara que se pueda cambiar de estado
            Debug.Log(gameObject.name + ": vuelve a patrullar.");
        }
    }

    
    private void OnDrawGizmos()
    {
        if (Camino == null || Camino.Count == 0)
            return;

        Gizmos.color = Color.cyan;

        foreach (Nodo nodo in Camino)
        {
            Gizmos.DrawCube(
                nodo.posicionMundo,
                new Vector3(0.8f, 0.8f, 0.8f)
            );
        }
    }

}

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
         // Estado inicial: seguir al jefe.
        FSM.RegisterState(new LeaderFollowingState(this));

        // Estados de persecución cuando aparece el jugador.
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));
        FSM.RegisterState(new ZombiePursuitState(this));

        // Empezamos siguiendo al jefe, no patrullando.
        FSM.SetState(typeof(LeaderFollowingState));

    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        //que el zombie patrulle y que quien haya entrado sea la capsula del jugador
        if (FSM.CurrentState is LeaderFollowingState  && otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            int eleccion = Random.Range(0, 2);  //para obtener un 0 o un 1

            if (eleccion == 0)
                FSM.TriggerEvent("SeekChosen");  //si es 0 entonces seek, hara chase
            else
                FSM.TriggerEvent("PursuitChosen");  //si es 1 entonces pursuit

            Debug.Log(gameObject.name + " -> " + FSM.CurrentState.GetType().Name);  //para ver en la consola que ha tocado
        }
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        //si la capsula del jugador salen del rango del zombie
        if (otroCollider.CompareTag("Player") && otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerLost");  //cambiamos de etiqueta que hara que se pueda cambiar de estado
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

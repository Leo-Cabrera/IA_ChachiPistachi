using UnityEngine;

public class SpiritController : NpcController
{
    public float velocidad = 5f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float arriveDistance = .4f;
    public Vector3 PosicionAlerta { get; private set; }

    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)

        //creamos el estado de patrulla del espiritu
        PatrolState patrullaSpirit = new PatrolState(this, velocidad, RotacionVel);  //para que adopte su velocidad inicial

        //esta ser� la regla de transici�n de patrulla a hacia el lugar del grito
        patrullaSpirit.ConfigurarTransicion("AlertReceived", typeof(GoToAlertState));  

        //registramos en la maquina de estados este estado teniendo en cuenta su transicion
        FSM.RegisterState(patrullaSpirit);

        //registramos ahora el estado de ir hacia donde se encuentra el chillido de la armadura
        FSM.RegisterState(new GoToAlertState(this, velocidad, RotacionVel));

        //registramos ahora el estado de investigar si esta el jugador una vez que ha llegado donde el chillido
        FSM.RegisterState(new InvestigateState(this));

        //registramos ahora el estado de persecucion
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        //comienza con el estado de patrulla
        FSM.SetState(typeof(PatrolState));
    }

    //metodo que usar� armor cuando sea necesario
    public void OnAlerted(Vector3 posicionGrito) //se le pasar� la posici�n de la armadura que haya pegado el grito
    {
        PosicionAlerta = posicionGrito;
        FSM.TriggerEvent("AlertReceived");  //asi la fsm recibe el cambio de evento para que pueda pasar a otro estado
        Debug.Log("Fantasma avisado. Estado: " + FSM.CurrentState.GetType().Name);
    }

    //OnTriggerStay m�todo que Unity llama repetidamente mientras otro collider permanece dentro de la zona trigger del fantasma
    //usamos este porque el jugador podr�a estar ya dentro de la zona cuando el fantasma empieza a investigar
    private void OnTriggerStay(Collider otroCollider)
    {
        //mientras haya maquina de estados, este en el estado investigar y el collider sea el de la capsula del jugador
        if (FSM != null && FSM.CurrentState is InvestigateState && otroCollider.gameObject.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerDetected");  //se ha detectado al jugador, se cambia de estado para poder hacer transicion
        }
    }

    private void OnTriggerExit(Collider otroCollider) //lo llama Unity cuando un collider sale de la zona trigger
    {
        //Debug.Log("Spirit: sale del trigger " + otroCollider.name
        //+ " (" + otroCollider.GetType().Name + "), estado: "
        //+ FSM.CurrentState.GetType().Name); 

        //hay maquina de estados, estamos en el estado de persecucion y a quien perseguimos era a la capsula del jugador (quien sale)
        if (FSM != null && FSM.CurrentState is ChaseState && otroCollider.gameObject.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerLost");  //entonces el jugador ha salido de la zona del fantasma, cambiamos el evento para cambiar estado
        }
    }

    private void OnDrawGizmos()
    {
        if (Camino == null || Camino.Count == 0)
            return;

        Gizmos.color = Color.white;

        for (int i = 0; i < Camino.Count - 1; i++)
        {
            Gizmos.DrawLine(
                Camino[i].posicionMundo,
                Camino[i + 1].posicionMundo
            );
        }
    }
}

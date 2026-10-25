using UnityEngine;
using System.Collections.Generic;

public class SpiritController : NpcController
{
    public float velocidad = 5f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float arriveDistance = .4f;
    public Vector3 PosicionAlerta { get; private set; }
    [SerializeField] private MiniGhostController miniGhostPrefab;
    [SerializeField] private Animator animadorSpirit;
    private Transform cabezaSpirit;

    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)

        //creamos el estado de patrulla del espiritu
        PatrolState patrullaSpirit = new PatrolState(this, velocidad, RotacionVel);  //para que adopte su velocidad inicial

        //esta sera la regla de transicion de patrulla a hacia el lugar del grito
        patrullaSpirit.ConfigurarTransicion("AlertReceived", typeof(GoToAlertState));  

        //registramos en la maquina de estados este estado teniendo en cuenta su transicion
        FSM.RegisterState(patrullaSpirit);

        //registramos ahora el estado de ir hacia donde se encuentra el chillido de la armadura
        FSM.RegisterState(new GoToAlertState(this, velocidad, RotacionVel));

        //registramos ahora el estado de investigar si esta el jugador una vez que ha llegado donde el chillido
        FSM.RegisterState(new InvestigateState(this));

        //creamos y registramos el estado para que la fsm lo conozca
        FSM.RegisterState(new SummonState(this));

        //registramos ahora el estado de persecucion
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        //comienza con el estado de patrulla
        FSM.SetState(typeof(PatrolState));
    }

    private void LateUpdate()
    {
        if (animadorSpirit == null || FSM == null)
            return;

        bool caminando =
            FSM.CurrentState is PatrolState ||
            FSM.CurrentState is GoToAlertState ||
            FSM.CurrentState is InvestigateState ||
            FSM.CurrentState is ChaseState;

        animadorSpirit.SetBool("Walk", caminando);

        if (FSM.CurrentState is ChaseState)
            animadorSpirit.speed = 1.5f;
        else if (FSM.CurrentState is InvestigateState)
            animadorSpirit.speed = 0.5f;
        else
            animadorSpirit.speed = 1f;

        if (FSM.CurrentState is InvestigateState && cabezaSpirit != null && animadorSpirit.isActiveAndEnabled)
        {
            float angulo = Mathf.Sin(Time.time * 2f) * 55f;
            cabezaSpirit.rotation =
                Quaternion.AngleAxis(angulo, transform.up) * cabezaSpirit.rotation;
        }
    }

    public void ReproducirInvocacion()
    {
        if (animadorSpirit == null)
            return;

        animadorSpirit.SetTrigger("Yell");
    }

    public void InvocarMinifantasmas()
    {
        Parilla parilla = FindFirstObjectByType<Parilla>();

        if (miniGhostPrefab == null || target == null || parilla == null)  //si no hay prefab o target
            return;

        Nodo nodoSpirit = parilla.ObtenerNodoDesdePosicion(transform.position);
        List<Nodo> vecinos = parilla.ObtenerVecinos(nodoSpirit);  //queremos guardarnos los nodos caminables vecinos a spirit

        List<Nodo> candidatos = new List<Nodo>(vecinos);

        foreach (Nodo vecino in vecinos)
        {
            foreach (Nodo otro in parilla.ObtenerVecinos(vecino))
            {
                if (otro != nodoSpirit && !candidatos.Contains(otro))
                    candidatos.Add(otro);
            }
        }

        int creados = 0;

        foreach (Nodo candidato in candidatos)
        {
            Vector3 posicion = candidato.posicionMundo;
            posicion.y = transform.position.y;

            if (Vector3.Distance(posicion, transform.position) < 1.4f)
                continue;

            MiniGhostController miniGhost = Instantiate(miniGhostPrefab, posicion, transform.rotation);

            miniGhost.AsignarJugador(target);
            creados++;

            if (creados == 3)
                break;
        }
    }

    //metodo que usara armor cuando sea necesario
    public void OnAlerted(Vector3 posicionGrito) //se le pasara la posicion de la armadura que haya pegado el grito
    {
        PosicionAlerta = posicionGrito;
        FSM.TriggerEvent("AlertReceived");  //asi la fsm recibe el cambio de evento para que pueda pasar a otro estado
        Debug.Log("Fantasma avisado. Estado: " + FSM.CurrentState.GetType().Name);
    }

    //OnTriggerStay metodo que Unity llama repetidamente mientras otro collider permanece dentro de la zona trigger del fantasma
    //usamos este porque el jugador podria estar ya dentro de la zona cuando el fantasma empieza a investigar
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

        foreach (Nodo nodo in Camino)
        {
            Gizmos.DrawCube(
                nodo.posicionMundo,
                new Vector3(0.8f, 0.8f, 0.8f)
            );
        }
    }

    private void Start()
    {
        if (animadorSpirit != null && animadorSpirit.avatar != null && animadorSpirit.avatar.isHuman)
            cabezaSpirit = animadorSpirit.GetBoneTransform(HumanBodyBones.Head);
    }

}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiritController : NpcController
{
    public float velocidad = 5f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;

    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float arriveDistance = 0.4f;

    public Vector3 PosicionAlerta { get; private set; }

    [SerializeField] private MiniGhostController miniGhostPrefab;
    [SerializeField] private Animator animadorSpirit;

    private Transform cabezaSpirit;

    private bool esperandoTrasEscondite = false;

    public bool EsperandoTrasEscondite => esperandoTrasEscondite;

    public void JugadorSeHaEscondido()
    {
        if (FSM == null || FSM.CurrentState == null)
            return;

        if (!(FSM.CurrentState is ChaseState) ||
            esperandoTrasEscondite)
            return;

        StartCoroutine(EsperarYVolverAPatrullar());
    }

    private IEnumerator EsperarYVolverAPatrullar()
    {
        esperandoTrasEscondite = true;

        Debug.Log("Spirit: jugador escondido. Espero 2 segundos.");

        yield return new WaitForSeconds(2f);

        if (FSM != null && FSM.CurrentState is ChaseState)
        {
            FSM.TriggerEvent("PlayerLost");
        }

        esperandoTrasEscondite = false;
    }

    protected override void ConfigureStates()
    {
        PatrolState patrullaSpirit =
            new PatrolState(this, velocidad, RotacionVel);

        patrullaSpirit.ConfigurarTransicion(
            "AlertReceived",
            typeof(GoToAlertState)
        );

        FSM.RegisterState(patrullaSpirit);

        FSM.RegisterState(
            new GoToAlertState(this, velocidad, RotacionVel)
        );

        FSM.RegisterState(new InvestigateState(this));
        FSM.RegisterState(new SummonState(this));

        FSM.RegisterState(
            new ChaseState(this, velocidad, RotacionVel)
        );

        FSM.SetState(typeof(PatrolState));
    }

    private void LateUpdate()
    {
        if (animadorSpirit == null || FSM == null)
            return;

        bool caminando = !esperandoTrasEscondite && (FSM.CurrentState is PatrolState || FSM.CurrentState is GoToAlertState ||
            FSM.CurrentState is InvestigateState || FSM.CurrentState is ChaseState);

        animadorSpirit.SetBool("Walk", caminando);

        if (FSM.CurrentState is ChaseState)
            animadorSpirit.speed = 1.5f;
        else if (FSM.CurrentState is InvestigateState)
            animadorSpirit.speed = 0.5f;
        else
            animadorSpirit.speed = 1f;

        if (FSM.CurrentState is InvestigateState &&
            cabezaSpirit != null &&
            animadorSpirit.isActiveAndEnabled)
        {
            float angulo = Mathf.Sin(Time.time * 2f) * 55f;

            cabezaSpirit.rotation =
                Quaternion.AngleAxis(angulo, transform.up) *
                cabezaSpirit.rotation;
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

        if (miniGhostPrefab == null ||
            target == null ||
            parilla == null)
            return;

        Nodo nodoSpirit =
            parilla.ObtenerNodoDesdePosicion(transform.position);

        List<Nodo> vecinos = parilla.ObtenerVecinos(nodoSpirit);
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

            MiniGhostController miniGhost = Instantiate(
                miniGhostPrefab,
                posicion,
                transform.rotation
            );

            miniGhost.AsignarJugador(target);
            creados++;

            if (creados == 3)
                break;
        }
    }

    public void OnAlerted(Vector3 posicionGrito)
    {
        PosicionAlerta = posicionGrito;

        if (FSM == null)
            return;

        FSM.TriggerEvent("AlertReceived");

        Debug.Log(
            "Fantasma avisado. Estado: " +
            FSM.CurrentState.GetType().Name
        );
    }

    private void OnTriggerStay(Collider otroCollider)
    {
        if (FSM != null &&
            FSM.CurrentState is InvestigateState &&
            otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerDetected");
        }
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        if (FSM != null &&
            FSM.CurrentState is ChaseState &&
            otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerLost");
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
        if (animadorSpirit != null &&
            animadorSpirit.avatar != null &&
            animadorSpirit.avatar.isHuman)
        {
            cabezaSpirit =
                animadorSpirit.GetBoneTransform(HumanBodyBones.Head);
        }
    }
}

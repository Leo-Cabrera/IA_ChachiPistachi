
using UnityEngine;

public class ZombieController : NpcController
{
    public float velocidad = 3f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 0.5f;

    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = 0.4f;
    [SerializeField] private Animator animadorZombie;

    private Transform brazoIzquierdo;
    private Transform brazoDerecho;

    [Header("Seguimiento del jefe")]
    public Transform jefe;

    private Vector3 posicionCajaEscondite;

    public Vector3 PosicionCajaEscondite => posicionCajaEscondite;

    protected override void ConfigureStates()
    {
        FSM.RegisterState(new LeaderFollowingState(this));
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));
        FSM.RegisterState(new ZombiePursuitState(this));
        FSM.RegisterState(new PatrolState(this, velocidad, RotacionVel));
        FSM.RegisterState(new ZombieFleeFromBoxState(this));

        FSM.SetState(typeof(LeaderFollowingState));
    }

    private void LateUpdate()
    {
        if (animadorZombie == null || FSM == null)
            return;

        bool caminando =
            FSM.CurrentState is LeaderFollowingState ||
            FSM.CurrentState is PatrolState ||
            FSM.CurrentState is ChaseState ||
            FSM.CurrentState is ZombiePursuitState ||
            FSM.CurrentState is ZombieFleeFromBoxState;

        animadorZombie.SetBool("Walk", caminando);

        bool persiguiendo =
            FSM.CurrentState is ChaseState ||
            FSM.CurrentState is ZombiePursuitState;

        if (persiguiendo &&
            brazoIzquierdo != null &&
            brazoDerecho != null &&
            animadorZombie.isActiveAndEnabled)
        {
            Quaternion giroBrazos =
                Quaternion.AngleAxis(-80f, transform.right);

            Quaternion izquierdoHaciaCentro =
                Quaternion.AngleAxis(20f, transform.up);

            Quaternion derechoHaciaCentro =
                Quaternion.AngleAxis(-20f, transform.up);

            brazoIzquierdo.rotation =
                izquierdoHaciaCentro * giroBrazos *
                brazoIzquierdo.rotation;

            brazoDerecho.rotation =
                derechoHaciaCentro * giroBrazos *
                brazoDerecho.rotation;
        }
    }

    private void Start()
    {
        if (animadorZombie == null ||
            animadorZombie.avatar == null ||
            !animadorZombie.avatar.isHuman)
            return;

        brazoIzquierdo =
            animadorZombie.GetBoneTransform(HumanBodyBones.LeftUpperArm);

        brazoDerecho =
            animadorZombie.GetBoneTransform(HumanBodyBones.RightUpperArm);
    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        if (FSM == null || FSM.CurrentState == null)
            return;

        if ((FSM.CurrentState is LeaderFollowingState ||
             FSM.CurrentState is PatrolState) &&
            otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            int eleccion = Random.Range(0, 2);

            if (eleccion == 0)
                FSM.TriggerEvent("SeekChosen");
            else
                FSM.TriggerEvent("PursuitChosen");

            Debug.Log(
                gameObject.name + " -> " +
                FSM.CurrentState.GetType().Name
            );
        }

        if (FSM.CurrentState is PatrolState &&
            jefe != null &&
            otroCollider.transform == jefe)
        {
            FSM.TriggerEvent("FollowBoss");
            Debug.Log(gameObject.name + ": vuelve a seguir al jefe.");
        }
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        if (FSM == null || FSM.CurrentState == null)
            return;

        if (otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider &&
            (FSM.CurrentState is ChaseState ||
             FSM.CurrentState is ZombiePursuitState))
        {
            FSM.TriggerEvent("PlayerLost");
            Debug.Log(gameObject.name + ": vuelve a patrullar.");
        }
    }

    private void OnTriggerStay(Collider otroCollider)
    {
        if (FSM == null || FSM.CurrentState == null)
            return;

        if (FSM.CurrentState is PatrolState &&
            jefe != null &&
            otroCollider.transform.root == jefe.root)
        {
            FSM.TriggerEvent("FollowBoss");
            Debug.Log(gameObject.name + ": vuelve a seguir al jefe.");
        }
    }

    // Esconderse en el armario: abandono inmediato de la persecución.
    public void JugadorSeHaEscondido()
    {
        if (FSM == null || FSM.CurrentState == null)
            return;

        if (!(FSM.CurrentState is ChaseState) &&
            !(FSM.CurrentState is ZombiePursuitState))
            return;

        ReiniciarCamino();
        FSM.SetState(typeof(PatrolState));

        Debug.Log(
            gameObject.name +
            ": jugador escondido; vuelvo a patrullar inmediatamente."
        );
    }

    // Mecánica independiente: huir de la caja.
    public void AvisarEscondite(Vector3 posicionCaja)
    {
        posicionCajaEscondite = posicionCaja;

        if (FSM == null || FSM.CurrentState == null)
            return;

        if (FSM.CurrentState is ChaseState ||
            FSM.CurrentState is ZombiePursuitState)
        {
            FSM.SetState(typeof(ZombieFleeFromBoxState));

            Debug.Log(gameObject.name + ": se aleja de la caja.");
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

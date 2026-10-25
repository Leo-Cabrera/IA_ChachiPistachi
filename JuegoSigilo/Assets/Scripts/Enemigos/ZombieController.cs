using UnityEngine;

public class ZombieController : NpcController
{
    public float velocidad = 3f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 0.5f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = .4f;
    [SerializeField] private Animator animadorZombie;
    private Transform brazoIzquierdo;
    private Transform brazoDerecho;

    [Header("Seguimiento del jefe")]
    public Transform jefe;

    private Vector3 posicionCajaEscondite;

    public Vector3 PosicionCajaEscondite => posicionCajaEscondite;

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

        FSM.RegisterState(new ZombieFleeFromBoxState(this));
        
        // Empezamos siguiendo al jefe
        FSM.SetState(typeof(LeaderFollowingState));

        

    }

    private void LateUpdate()
    {
        if (animadorZombie == null || FSM == null)
            return;

        bool caminando = FSM.CurrentState is LeaderFollowingState || FSM.CurrentState is PatrolState || FSM.CurrentState is ChaseState || FSM.CurrentState is ZombiePursuitState || FSM.CurrentState is ZombieFleeFromBoxState;

        animadorZombie.SetBool("Walk", caminando);

        bool persiguiendo = FSM.CurrentState is ChaseState || FSM.CurrentState is ZombiePursuitState;

        if (persiguiendo && brazoIzquierdo != null && brazoDerecho != null && animadorZombie.isActiveAndEnabled)
        {
            Quaternion giroBrazos = Quaternion.AngleAxis(-80f, transform.right);
            Quaternion izquierdoHaciaCentro = Quaternion.AngleAxis(20f, transform.up);
            Quaternion derechoHaciaCentro = Quaternion.AngleAxis(-20f, transform.up);

            brazoIzquierdo.rotation = izquierdoHaciaCentro * giroBrazos * brazoIzquierdo.rotation;
            brazoDerecho.rotation = derechoHaciaCentro * giroBrazos * brazoDerecho.rotation;
        }
    }

    private void Start()
    {
        if (animadorZombie == null || animadorZombie.avatar == null || !animadorZombie.avatar.isHuman)
            return;

        brazoIzquierdo = animadorZombie.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        brazoDerecho = animadorZombie.GetBoneTransform(HumanBodyBones.RightUpperArm);
    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        //Si entra el jugador mientras seguimos al jefe
        if ((FSM.CurrentState is LeaderFollowingState || FSM.CurrentState is PatrolState)&& otroCollider.CompareTag("Player")&& otroCollider is CapsuleCollider)
        {
            int eleccion = Random.Range(0, 2);  //para obtener un 0 o un 1

            if (eleccion == 0)
                FSM.TriggerEvent("SeekChosen");  //si es 0 entonces seek, hara chase
            else
                FSM.TriggerEvent("PursuitChosen");  //si es 1 entonces pursuit

            Debug.Log(gameObject.name + " -> " + FSM.CurrentState.GetType().Name);  //para ver en la consola que ha tocado
        }

        //Si es el jefe mientras patrulla, pasamos a seguirlo
        if (FSM.CurrentState is PatrolState && otroCollider.transform == jefe)
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

    private void OnTriggerStay(Collider otroCollider)
    {
        if (FSM.CurrentState is PatrolState
            && jefe != null
            && otroCollider.transform.root == jefe.root)
        {
            FSM.TriggerEvent("FollowBoss");
            Debug.Log(gameObject.name + ": vuelve a seguir al jefe.");
        }
    }


    public void AvisarEscondite(Vector3 posicionCaja)
    {
        posicionCajaEscondite = posicionCaja;

        if (FSM.CurrentState is ChaseState|| FSM.CurrentState is ZombiePursuitState)
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

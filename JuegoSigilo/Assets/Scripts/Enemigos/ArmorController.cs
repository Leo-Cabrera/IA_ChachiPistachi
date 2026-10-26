
using UnityEngine;
using System.Collections;

public class ArmorController : NpcController
{
    public float velocidad = 1f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    public float defenseTime = 10f;
    public float defenseCrono = 0f;

    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float arriveDistance = .4f;
    [SerializeField] private float radioDeteccion = 10f;

    public AudioSource alertSoundSource;
    public bool withYaiko;
    [SerializeField] public Transform yaikoPos;
    [SerializeField] public Animator armorAnimator;

    private Transform cabezaArmor;

    private bool esperandoTrasEscondite = false;
    public bool EsperandoTrasEscondite => esperandoTrasEscondite;

    public void JugadorSeHaEscondido()
    {
        if (FSM == null ||
            !(FSM.CurrentState is ChaseState) ||
            esperandoTrasEscondite)
        {
            return;
        }

        StartCoroutine(EsperarYVolverAPatrullar());
    }

    private IEnumerator EsperarYVolverAPatrullar()
    {
        esperandoTrasEscondite = true;

        Debug.Log("[ARMOR] Empieza la espera de 2 segundos.");

        yield return new WaitForSeconds(2f);

        if (FSM == null)
        {
            Debug.LogError("[ARMOR] FSM es null.");
            esperandoTrasEscondite = false;
            yield break;
        }

        Debug.Log("[ARMOR] Estado después de esperar: " +
            (FSM.CurrentState != null
                ? FSM.CurrentState.GetType().Name
                : "null"));

        if (FSM.CurrentState is ChaseState)
        {
            FSM.TriggerEvent("PlayerHidden");

            Debug.Log("[ARMOR] Estado después del evento: " +
                (FSM.CurrentState != null
                    ? FSM.CurrentState.GetType().Name
                    : "null"));
        }

        esperandoTrasEscondite = false;
    }

    private void Start()
    {
        if (alertSoundSource != null)
        {
            alertSoundSource.volume =
                PlayerPrefs.GetFloat("SFXVolume", 0.3f);
        }

        if (armorAnimator != null &&
            armorAnimator.avatar != null &&
            armorAnimator.avatar.isHuman)
        {
            cabezaArmor =
                armorAnimator.GetBoneTransform(HumanBodyBones.Head);
        }
    }

    private void LateUpdate()
    {
        if (armorAnimator == null || FSM == null)
            return;

        if (FSM.CurrentState is DefenseState)
        {
            if (cabezaArmor == null &&
                armorAnimator.isActiveAndEnabled &&
                armorAnimator.avatar != null &&
                armorAnimator.avatar.isHuman)
            {
                cabezaArmor =
                    armorAnimator.GetBoneTransform(HumanBodyBones.Head);
            }

            if (cabezaArmor != null && armorAnimator.isActiveAndEnabled)
            {
                float angulo = Mathf.Sin(Time.time * 2f) * 65f;
                cabezaArmor.localRotation =
                    Quaternion.Euler(0f, angulo, 0f);
            }

            if (Time.frameCount % 60 == 0)
            {
                Debug.Log("Armor Defense | cabeza: " +
                    (cabezaArmor != null ? cabezaArmor.name : "null") +
                    " | Animator activo: " +
                    armorAnimator.isActiveAndEnabled);
            }

            return;
        }

        bool caminando = !esperandoTrasEscondite && (FSM.CurrentState is PatrolState || FSM.CurrentState is ChaseState);

        armorAnimator.SetBool("Walk", caminando);
    }

    protected override void ConfigureStates()
    {
        PatrolState patrullaArmor =
            new PatrolState(this, velocidad, RotacionVel);

        patrullaArmor.ConfigurarTransicion(
            "PlayerDetected", typeof(ChaseState));

        FSM.RegisterState(patrullaArmor);

        ChaseState perseguirArmor =
            new ChaseState(this, velocidad, RotacionVel);

        // La armadura pasa a defensa cuando pierde al jugador
        // o cuando termina la espera tras esconderse.
        perseguirArmor.ConfigurarTransicion(
            "PlayerHidden", typeof(DefenseState));

        FSM.RegisterState(perseguirArmor);

        DefenseState defend =
            new DefenseState(
                this, velocidad, RotacionVel,
                defenseTime, defenseCrono);

        defend.ConfigurarTransicion(
            "DefenseEnded", typeof(PatrolState));

        FSM.RegisterState(defend);

        FSM.SetState(typeof(PatrolState));
    }

    private void OnTriggerEnter(Collider otroCollider)
    {
        if (FSM == null)
            return;

        if (FSM.CurrentState is PatrolState &&
            otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            FSM.TriggerEvent("PlayerDetected");

            if (alertSoundSource != null)
            {
                alertSoundSource.volume =
                    PlayerPrefs.GetFloat("SFXVolume", 0.3f);

                alertSoundSource.Play();
            }

            AlertarFantasmasCercanos();
        }
    }

    private void OnTriggerStay(Collider otroCollider)
    {
        OnTriggerEnter(otroCollider);
    }

    private void OnTriggerExit(Collider otroCollider)
    {
        if (FSM == null)
            return;

        if (otroCollider.CompareTag("Player") &&
            otroCollider is CapsuleCollider)
        {
            Debug.Log("Jugador perdido");
            FSM.TriggerEvent("PlayerLost");
        }
    }

    private void AlertarFantasmasCercanos()
    {
        SpiritController[] fantasmas =
            FindObjectsByType<SpiritController>(
                FindObjectsSortMode.None);

        foreach (SpiritController fantasma in fantasmas)
        {
            if (fantasma == null)
                continue;

            float distancia = Vector3.Distance(
                transform.position, fantasma.transform.position);

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
                new Vector3(0.8f, 0.8f, 0.8f));
        }
    }

    public void EnterYaikoZone()
    {
        withYaiko = true;
    }

    public void ExitYaikoZone()
    {
        withYaiko = false;
    }

    public void EndDefense()
    {
        if (FSM != null)
            FSM.TriggerEvent("DefenseEnded");
    }
}

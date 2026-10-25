using UnityEngine;

public class MiniGhostController : NpcController
{
    //minima y maxima porque queremos que sea aleatorio entre ese rango
    [SerializeField] private float velocidadMinima = 2f;
    [SerializeField] private float velocidadMaxima = 4f;
    [SerializeField] private float duracionMinima = 3f;
    [SerializeField] private float duracionMaxima = 7f;

    public float Velocidad { get; private set; }   //guardaremos la velocidad que le toque
    public float TiempoLimite { get; private set; }  //tiempo maximo hasta que tenga que desaparecer

    protected override void ConfigureStates()
    {
        Velocidad = Random.Range(velocidadMinima, velocidadMaxima);  //sale velocidad aleatoria

        float duracionElegida = Random.Range(duracionMinima, duracionMaxima);  //duracion aleatoria
        TiempoLimite = Time.time + duracionElegida;  //para que al tiempo en donde se crea el fantasma se le sume el que le ha tocado

        //cuando termine su persecucion pasarán al estado de desaparecer
        ChaseState persecucion = new ChaseState(this, Velocidad, 5f); //crear un ChaseState usando la velocidad aleatoria de esta copia
        persecucion.ConfigurarTransicion("Disappear", typeof(DisappearState));
        FSM.RegisterState(persecucion);

        //registramos el estado de desaparecer para cuando muera el fantasmita
        FSM.RegisterState(new DisappearState(this));

        FSM.SetState(typeof(ChaseState));  //hacemos que empiece ya en chase desde que aparece
    }

    public void AsignarJugador(Transform jugador)
    {
        target = jugador;
    }

    private void LateUpdate()
    {
        if (FSM == null || !(FSM.CurrentState is ChaseState))
            return;

        if (Time.time >= TiempoLimite)  //si ya ha llegado hasta el limite del tiempo que debe vivir
        {
            FSM.TriggerEvent("Disappear");  //cambiamos evento, ha de desaparecer
            return;
        }

        //si el jugador está escondido, el fantasma ha de llegar a su ultima posicion, al no estar el jugador desaparece al llegar
        //tenemos target y el target esté inactivo y que se haya llegado hasta la posicion del target
        if (target != null && !target.gameObject.activeInHierarchy && EstaEnNodoDe(target.position))
        {
            FSM.TriggerEvent("Disappear");  //pasamos al estado de la desaparicion
        }
    }
}

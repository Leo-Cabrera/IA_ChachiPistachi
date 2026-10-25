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
    public Vector3 UltimaPosicion { get; set; }  //guardaremos la ultima posicion del jugador

    protected override void ConfigureStates()
    {
        Velocidad = Random.Range(velocidadMinima, velocidadMaxima);  //sale velocidad aleatoria

        float duracionElegida = Random.Range(duracionMinima, duracionMaxima);  //duracion aleatoria
        TiempoLimite = Time.time + duracionElegida;  //para que al tiempo en donde se crea el fantasma se le sume el que le ha tocado

        FSM.RegisterState(new ChaseState(this, Velocidad, 5f));  //crear un ChaseState usando la velocidad aleatoria de esta copia
        FSM.RegisterState(new DisappearState(this));  //registramos el estado de desaparecer para cuando muera el fantasmita
        FSM.SetState(typeof(ChaseState));  //hacemos que empiece ya en chase desde que aparece
    }

    public void AsignarJugador(Transform jugador)
    {
        target = jugador;
        UltimaPosicion = jugador.position;
    }
}

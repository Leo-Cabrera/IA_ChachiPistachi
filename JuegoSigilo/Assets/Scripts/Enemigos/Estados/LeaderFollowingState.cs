
using UnityEngine;

public class LeaderFollowingState : NpcState
{
    // Referencia al zombi normal que sigue al jefe.
    private ZombieController zombie;

    // Referencia al zombi jefe.
    private Transform jefe;

    // Distancia a la que queremos situarnos detrás del jefe.
    private float distanciaDetras = 2f;

    // Distancia mínima para considerar que hemos llegado al objetivo.
    private float distanciaLlegada = 0.5f;

    public LeaderFollowingState(ZombieController zombie) : base(zombie)
    {
        this.zombie = zombie;
        this.jefe = zombie.jefe;

         // Si el jugador entra en el rango, elegimos cómo perseguirlo.
        AddTransition("SeekChosen", typeof(ChaseState));
        AddTransition("PursuitChosen", typeof(ZombiePursuitState));
    }

    public override void Enter()
    {
        // Borramos cualquier camino anterior.
        zombie.ReiniciarCamino();

        Debug.Log(zombie.gameObject.name + ": sigue al jefe.");
    }

    public override void Execute()
    {
        // Si no tenemos asignado el jefe, no hacemos nada.
        if (jefe == null)
            return;

        // Calculamos una posición situada detrás del jefe.
        Vector3 posicionObjetivo = jefe.position - jefe.forward * distanciaDetras;

        // Ignoramos la altura para movernos por el suelo.
        posicionObjetivo.y = zombie.transform.position.y;

        // Si ya estamos cerca de la posición deseada, no seguimos avanzando.
        if (Vector3.Distance(zombie.transform.position, posicionObjetivo) <= distanciaLlegada)
        {
            zombie.ReiniciarCamino();
            return;
        }

        // Calculamos una ruta A* hacia la posición situada detrás del jefe.
        zombie.ActualizarCamino(posicionObjetivo);

        // Avanzamos por la ruta calculada, evitando las paredes según la cuadrícula.
        zombie.MoverPorCamino(zombie.velocidad, zombie.RotacionVel);
    }

    public override void Exit()
    {
        // Borramos la ruta cuando abandonamos este estado.
        zombie.ReiniciarCamino();
    }
}

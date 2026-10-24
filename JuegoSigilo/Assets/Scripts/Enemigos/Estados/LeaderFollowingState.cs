
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

   // Distancia mínima que intentamos mantener respecto a otros zombis.
    private float distanciaSeparacion = 1.5f;

    // Fuerza con la que evitamos acercarnos demasiado a otros zombis.
    private float fuerzaSeparacion = 2f;

    // Radio en el que buscamos otros zombis.
    private float radioSeparacion = 3f;

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

        // Calculamos la dirección para separarnos de otros zombis.
        Vector3 separacion = CalcularSeparacion();

        // Desplazamos el objetivo para evitar amontonarnos.
        posicionObjetivo += separacion * fuerzaSeparacion;

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


    // Calcula una dirección para alejarse de los zombis cercanos.
    private Vector3 CalcularSeparacion()
    {
        // Buscamos todos los zombis de la escena.
        ZombieController[] zombis = Object.FindObjectsByType<ZombieController>(FindObjectsSortMode.None);

        Vector3 direccionSeparacion = Vector3.zero;
        int cantidadZombis = 0;

        foreach (ZombieController otroZombi in zombis)
        {
            // No nos tenemos en cuenta a nosotros mismos.
            if (otroZombi == zombie)
                continue;

            // Calculamos la distancia horizontal entre ambos zombis.
            Vector3 diferencia = zombie.transform.position - otroZombi.transform.position;

            diferencia.y = 0f;

            float distancia = diferencia.magnitude;

            // Solo nos alejamos de los zombis que están cerca.
            if (distancia < radioSeparacion && distancia > 0.01f)
            {
                // Cuanto más cerca esté el otro zombi, mayor será la separación.
                direccionSeparacion += diferencia.normalized / distancia;
                cantidadZombis++;
            }
        }

        // Si hay zombis cerca, calculamos la dirección de separación.
        if (cantidadZombis > 0)
        {
            direccionSeparacion /= cantidadZombis;

            if (direccionSeparacion.sqrMagnitude > 0.001f)
                direccionSeparacion.Normalize();
        }

        return direccionSeparacion;
    }
}

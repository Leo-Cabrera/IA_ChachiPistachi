using UnityEngine;

public class ZombieFleeFromBoxState : NpcState
{
    private ZombieController zombie;
    private Vector3 destino;
    private const float distanciaEscape = 5f;
    private const float distanciaLlegada = 0.5f;

    public ZombieFleeFromBoxState(ZombieController zombie) : base(zombie)
    {
        this.zombie = zombie;
        AddTransition("EscapeFinished", typeof(PatrolState));
    }

    public override void Enter()
    {
        zombie.ReiniciarCamino();

        Vector3 direccionAlejamiento =
            zombie.transform.position - zombie.PosicionCajaEscondite;

        direccionAlejamiento.y = 0f;

        if (direccionAlejamiento.sqrMagnitude < 0.001f)
            direccionAlejamiento = -zombie.transform.forward;

        destino = zombie.transform.position
            + direccionAlejamiento.normalized * distanciaEscape;

        destino.y = zombie.transform.position.y;
    }

    public override void Execute()
    {
        if (Vector3.Distance(zombie.transform.position, destino) <= distanciaLlegada)
        {
            zombie.FSM.TriggerEvent("EscapeFinished");
            return;
        }

        zombie.ActualizarCamino(destino);
        zombie.MoverPorCamino(zombie.velocidad, zombie.RotacionVel);
    }

    public override void Exit()
    {
        zombie.ReiniciarCamino();
    }
}

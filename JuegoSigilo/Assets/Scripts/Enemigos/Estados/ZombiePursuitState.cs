
using UnityEngine;

public class ZombiePursuitState : NpcState
{
    private ZombieController zombie;

    public ZombiePursuitState(ZombieController zombie) : base(zombie)
    {
        this.zombie = zombie;

        AddTransition("PlayerLost", typeof(PatrolState));
    }

    public override void Enter()
    {
        zombie.ReiniciarCamino();
    }

    public override void Execute()
    {
        if (zombie.target == null)
            return;

        Rigidbody rbJugador =
            zombie.target.GetComponent<Rigidbody>();

        Vector3 velocidadJugador = Vector3.zero;

        if (rbJugador != null)
            velocidadJugador = rbJugador.linearVelocity;

        Vector3 posicionPredicha =
            zombie.target.position +
            velocidadJugador * zombie.tiempoPrediccion;

        zombie.ActualizarCamino(posicionPredicha);

        zombie.MoverPorCamino(
            zombie.velocidad,
            zombie.RotacionVel
        );
    }

    public override void Exit()
    {
        zombie.ReiniciarCamino();
    }
}

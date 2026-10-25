using UnityEngine;

public class ZombiePursuitState : NpcState
{
    private ZombieController zombie;

    public ZombiePursuitState(ZombieController zombie) : base(zombie)  //pillamos la base npc del padre npcstate
    {
        this.zombie = zombie;
        // Si pierde al jugador, vuelve a seguir al jefe.
        AddTransition("PlayerLost", typeof(PatrolState));
    }

    public override void Enter()  
    {
        zombie.ReiniciarCamino();  //al entrar en este estado borra la ruta anterior guardada que tenia el zombi al patrullar
    } 

    public override void Execute()
    {
        if (zombie.target == null)  //para evitar que salga error si el zombie no tiene target
            return;

        Rigidbody rbJugador = zombie.target.GetComponent<Rigidbody>();  //comprueba si existe el componente Rigidbody

        Vector3 velocidadJugador = Vector3.zero;  //asumimos de momento que esta quieto el jugador y ponemos que tiene 0 en cada vector

        if (rbJugador != null)  //si hemos encontrado su velocidad
            velocidadJugador = rbJugador.linearVelocity;  //la guardamos en el vector adecuado

        //esto estimara el donde se encontrara el jugador
        Vector3 posicionPredicha = zombie.target.position + velocidadJugador * zombie.tiempoPrediccion;

        //posicionPredicha.y = zombie.transform.position.y;

        zombie.ActualizarCamino(posicionPredicha);
        zombie.MoverPorCamino(zombie.velocidad, zombie.RotacionVel);
    }

    public override void Exit()
    {
        zombie.ReiniciarCamino();  //al salir de este estado, borra la ruta que tuviera en el 
    }
}

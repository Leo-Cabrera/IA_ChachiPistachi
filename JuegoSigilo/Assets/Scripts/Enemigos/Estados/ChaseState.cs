using UnityEngine;

//estado de perseguir que tiene que heredar la estructura comun de los estados npc 
public class ChaseState : NpcState
{
    private float velocidad;
    private float rotacionVel;

    //se ejecutara al crear el estado y le pasamos noc de la clase que hereda 
    public ChaseState(NpcController npc, float velocidad, float rotacionVel) : base(npc)
    {
        this.velocidad = velocidad;
        this.rotacionVel = rotacionVel;

        AddTransition("PlayerLost", typeof(PatrolState));
        
    }

    //se ejecuta cada vez que el npc entra en el estado de persecucion
    public override void Enter()
    {
        npc.ReiniciarCamino();  //cuando entre en chase tendra que recalcular su ruta para perseguir
        Debug.Log("NPC: comienzo a perseguir.");
    }

    //se ejecuta en cada fotograma mientras dure este estado
    public override void Execute()
    {
        if (npc.target == null)  //si no existe la referencia del jugador se termina
            return;

        //la posicion actual del jugador e la pasamos a ActualizarCamino, que calcula una ruta nueva si es necesario
        npc.ActualizarCamino(npc.target.position);

        //usa la ruta guardada y las velocidades de este enemigo para avanzar hacia el siguiente nodo
        npc.MoverPorCamino(velocidad, rotacionVel);
    }

    //se ejecuta en el momento en que salga del estado de persecucion
    public override void Exit()
    {
        npc.ReiniciarCamino(); //cuando salga de chase descartamos la ruta de persecucion
        Debug.Log(npc.gameObject.name + ": dejo de perseguir.");
    }
}

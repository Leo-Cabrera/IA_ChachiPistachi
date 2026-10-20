using UnityEngine;

public class ZombiePursuitState : NpcState
{
    //plantilla basica para empezar el estado del pursuit

    public ZombiePursuitState(ZombieController zombie) : base(zombie)  //pillamos la base npc del padre npcstate
    {
        //meteremos las cosas del constructor y las transiciones y tal
    }

    public override void Execute()
    {
        //lo que ira haciendo este npc en este estado
    }
}

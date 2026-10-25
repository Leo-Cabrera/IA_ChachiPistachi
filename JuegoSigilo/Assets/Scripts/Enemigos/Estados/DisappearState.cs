using UnityEngine;

public class DisappearState : NpcState
{
    public DisappearState(MiniGhostController miniGhost) : base(miniGhost)
    {
    }

    public override void Enter()
    {
        Object.Destroy(npc.gameObject);
    }

    public override void Execute()
    {
    }

    //no necesitaremos transiciones de cuando salgamos de este estado porque el fantasma desaparecerá y no mantendremos nada de él
}

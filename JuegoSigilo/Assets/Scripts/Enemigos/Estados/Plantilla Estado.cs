using UnityEngine;

public class PatrolState : NpcState
{
    // Índice del punto al que se dirige el NPC.
    private int currentPointIndex = 0;

    public PatrolState(NpcController npc) : base(npc)
    {
        // Si recibimos este evento, pasamos a perseguir.
        AddTransition("PlayerDetected", typeof(ChaseState));
    }

    public override void Enter()
    {
        Debug.Log("NPC: comienzo a patrullar.");
    }

    public override void Execute()
    {
        //update del metodo
    }

    public override void Exit()
    {
        Debug.Log("NPC: dejo de patrullar.");
    }

    //metodos complementarios
}

using UnityEngine;

public class SummonState : NpcState
{

    public SummonState(SpiritController spirit) : base(spirit)
    {
        AddTransition("SummonFinished", typeof(ChaseState));
    }

    public override void Enter()
    {
        Debug.Log("Spirit: entra en SummonState");
    }

    public override void Execute()
    {
        npc.FSM.TriggerEvent("SummonFinished");
    }

}

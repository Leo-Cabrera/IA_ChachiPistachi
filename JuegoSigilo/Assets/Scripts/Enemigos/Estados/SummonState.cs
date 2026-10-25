using UnityEngine;

public class SummonState : NpcState
{
    private float tiempoEnEstado;  //para que antes de cambiar de estado permanezca en este medio segundo
    private SpiritController spirit;

    public SummonState(SpiritController spirit) : base(spirit)
    {
        this.spirit = spirit;
        AddTransition("SummonFinished", typeof(ChaseState));
    }

    public override void Enter()
    {
        tiempoEnEstado = 0f;  //siempre que entremos en este estado, el tiempo se reinicia hasta cambiar al siguiente se reinicia
        Debug.Log("Spirit: entra en SummonState");
    }

    public override void Execute()
    {
        tiempoEnEstado += Time.deltaTime;

        if (tiempoEnEstado < 0.5f)
            return;

        spirit.InvocarMinifantasmas();
        npc.FSM.TriggerEvent("SummonFinished");
    }

}

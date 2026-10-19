using UnityEngine;

public class InvestigateState : NpcState
{
    private float tiempoTranscurrido;  //guarda cuantos segundos lleva de investigacion

    public InvestigateState(SpiritController spirit) : base(spirit)
    {
        //le ponemos estas reglas de estado
        AddTransition("PlayerDetected", typeof(ChaseState));
        AddTransition("AlertReceived", typeof(GoToAlertState));
        AddTransition("InvestigationFinished", typeof(PatrolState));
    }

    public override void Enter()
    {
        tiempoTranscurrido = 0f;  //iniciamos el tiempo de la investigacion en 0
    }

    public override void Execute()
    {
        tiempoTranscurrido += Time.deltaTime;  //una vez iniciado este estado vamos sumando al tiempo

        if (tiempoTranscurrido >= 10f)  //cuando hayan pasado 10 segundos, si no se ha detectado al jugador cambiaremos de estado
        {
            npc.FSM.TriggerEvent("InvestigationFinished");
        }
    }
}

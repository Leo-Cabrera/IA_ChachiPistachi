using UnityEngine;

public class InvestigateState : NpcState
{
    private float tiempoTranscurrido;  //guarda cuantos segundos lleva de investigacion
    private SpiritController spirit;  //para guardar la referencia del fantasma que estara usando este estado

    public InvestigateState(SpiritController spirit) : base(spirit)
    {
        this.spirit = spirit;

        //le ponemos estas reglas de estado
        AddTransition("PlayerDetected", typeof(SummonState));
        AddTransition("AlertReceived", typeof(GoToAlertState));
        AddTransition("InvestigationFinished", typeof(PatrolState));
    }

    public override void Enter()
    {
        tiempoTranscurrido = 0f;  //iniciamos el tiempo de la investigacion en 0
        spirit.crono = 0f;  //contador para cambiar de direccion cada cierto tiempo
        spirit.EligirNuevaDireccion();  //elige hacia donde empezara a caminar, su nueva direccion
        spirit.ReiniciarCamino();  //descarta la ruta anterior (la de llegar al grito)
    }

    public override void Execute()
    {
        tiempoTranscurrido += Time.deltaTime;  //una vez iniciado este estado vamos sumando al tiempo

        if (tiempoTranscurrido >= 10f)  //cuando hayan pasado 10 segundos, si no se ha detectado al jugador cambiaremos de estado
        {
            npc.FSM.TriggerEvent("InvestigationFinished");
            return;  //ya habremos salido de investigacion
        }


        //vamos a usar el mismo tipo de movimiento que tiene patrulla, para que pueda moverse, sin embargo aqui el fantasma
        //si que podra entrar en chase si ve al jugador

        spirit.crono += Time.deltaTime;  //cuenta cuanto lleva caminando en la direccion actual

        if (spirit.crono >= spirit.TiempoCambio)  //al llegar al tiempo del cambio de direccion
        {
            spirit.EligirNuevaDireccion();  //elige una nueva direccion
            spirit.crono = 0f;  //pone el contador que se usa para cambiar a 0
        }

        //calcula hacia donde debe mirar
        Quaternion rotacionDeseada = Quaternion.LookRotation(spirit.direccionRandom);

        //con slerp hacemos que gira gradualmente hacia la nueva direccion en que mira
        spirit.transform.rotation = Quaternion.Slerp(spirit.transform.rotation, rotacionDeseada, spirit.RotacionVel * Time.deltaTime);

        //hace que avance hacia la nueva direccion en la que mira
        spirit.transform.position += spirit.transform.forward * spirit.velocidad * Time.deltaTime;
    }
}

using UnityEngine;

public class GoToAlertState : NpcState
{
    //variable de tipo SpiritController porque este estado necesita leer PosicionAlerta(solo existe en el controlador del fantasma)
    private SpiritController spirit;  
    private float velocidad;
    private float rotacionVel;

    public GoToAlertState(SpiritController spirit, float velocidad, float rotacionVel) : base(spirit) 
    {
        this.spirit = spirit;
        this.velocidad = velocidad;
        this.rotacionVel = rotacionVel;
        AddTransition("AlertArrived", typeof(InvestigateState)); //cuando llegue al lugar del grito, su estado ha de cambiar a investigar
    }

    public override void Enter()  //se ejecuta una vez al comenzamiento
    {
        spirit.ReiniciarCamino();  //aquí simplemnete llama para empezar a tener ya una ruta
    }

    public override void Execute()
    {
        if (spirit.EstaEnNodoDe(spirit.PosicionAlerta))  //comrprueba en cada fotograma si ya esta en el nodo del grito
        {
            spirit.FSM.TriggerEvent("AlertArrived");  //cuando ya se entre al if, se cambiara su estado para poder hacer la transicion
            return;
        }

        //si no entra al if, se irán ejecutando estas lineas
        spirit.ActualizarCamino(spirit.PosicionAlerta);  //actualiza la ruta para llegar hasta la posicion del sonido
        spirit.MoverPorCamino(velocidad, rotacionVel);  //mueve al fantasma por la ruta
    }

    public override void Exit()
    {
        spirit.ReiniciarCamino();  //al salir de este estado, borra su ruta anterior
    }
}

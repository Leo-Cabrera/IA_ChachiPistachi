using UnityEngine;
using System;

public class PatrolState : NpcState
{
    // Indice del punto al que se dirige el NPC.
    private int currentPointIndex = 0;
    private float velocidad;
    private float rotacionVel;

    public PatrolState(NpcController npc, float velocidad, float rotacionVel) : base(npc)
    {
        // Si recibimos este evento, pasamos a perseguir.
        //AddTransition("PlayerDetected", typeof(ChaseState));

        this.velocidad = velocidad;
        this.rotacionVel = rotacionVel;
    }

    //recibe el evento que ocurrira y el estado al que pasara
    public void ConfigurarTransicion(string evento, Type siguienteEstado)
    {
        //se utilizara cuando pasemos de un estado al siguiente debido al evento
        AddTransition(evento, siguienteEstado);
    }

    public override void Enter()
    {
        npc.crono = 0f; //crono es nuestro contador de tiempo que usaremos para decidir cuando cambiar direccion
        npc.EligirNuevaDireccion();
        Debug.Log("NPC: comienzo a patrullar.");
    }

    public override void Execute()
    {
        //update del metodo
        npc.crono += Time.deltaTime; //suma el tiempo que esta pasando tras cada fotograma

        if (npc.crono >= npc.TiempoCambio)  //si han pasado los segundos necesarios para que haya un cambio de direccion (hemos puesto 4)
        {
            npc.EligirNuevaDireccion();  //elegimos nueva direccion
            npc.crono = 0f;  //reiniciamos el tiempo para que haya otro cambio
        }


        //Unity usa el Quaternion para guardar los giros 3D por eso lo necesitamos
        Quaternion rotacionDeseada = Quaternion.LookRotation(npc.direccionRandom);
        //guardamos en la varibale la orientacion de la nueva direccion (lookrotation ha convertido la direccion random, hacia donde mira)

        npc.MoverPatrullando(velocidad, rotacionVel);
    }

    public override void Exit()
    {
        Debug.Log("NPC: dejo de patrullar.");
    }

    //metodos complementarios
}

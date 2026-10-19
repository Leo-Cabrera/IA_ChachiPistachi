using UnityEngine;
using System;

public class PatrolState : NpcState
{
    // Índice del punto al que se dirige el NPC.
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

    //recibe el evento que ocurrirá y el estado al que pasará
    public void ConfigurarTransicion(string evento, Type siguienteEstado)
    {
        //se utilizará cuando pasemos de un estado al siguiente debido al evento
        AddTransition(evento, siguienteEstado);
    }

    public override void Enter()
    {
        npc.crono = 0f; //crono es nuestro contador de tiempo que usaremos para decidir cuándo cambiar dirección
        npc.EligirNuevaDireccion();
        Debug.Log("NPC: comienzo a patrullar.");
    }

    public override void Execute()
    {
        //update del metodo
        npc.crono += Time.deltaTime; //suma el tiempo que está pasando tras cada fotograma

        if (npc.crono >= npc.TiempoCambio)  //si han pasado los segundos necesarios para que haya un cambio de direccion (hemos puesto 4)
        {
            npc.EligirNuevaDireccion();  //elegimos nueva direccion
            npc.crono = 0f;  //reiniciamos el tiempo para que haya otro cambio
        }


        //Unity usa el Quaternion para guardar los giros 3D por eso lo necesitamos
        Quaternion rotacionDeseada = Quaternion.LookRotation(npc.direccionRandom);
        //guardamos en la varibale la orientación de la nueva dirección (lookrotation ha convertido la direccion random, hacia dónde mira)

        //esto cambiará la rotación del enemigo gracias a lo que ya hemos guardado antes
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);
        //Slerp calcula una orientación intermedia entre la actual y la deseada, por eso vamos dándole datos, la rotación que tiene ahora
        //el npc, la que queremos que tenga, y con el calculo de la velocidad haremos cuánto queremos que gire por cada fotograma, 
        //de esta manera no tendrá un giro brusco sino uno más progresivo entre fotogramas

        //Sumamos desde su posición actual el desplazamiento que va a tener
        npc.transform.position += npc.transform.forward * velocidad * Time.deltaTime;
        //irá hacia delante según su nueva posición al girarse, y con la velocidad y delta se calcula la distancia que recorre por
        //cada fotograma, así va progresivamente. Se suma a su posición actual para que pueda esta cambiar
    }

    public override void Exit()
    {
        Debug.Log("NPC: dejo de patrullar.");
    }

    //metodos complementarios
}

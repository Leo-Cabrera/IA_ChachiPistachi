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

        //esto cambiara la rotacion del enemigo gracias a lo que ya hemos guardado antes
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotacionDeseada, rotacionVel * Time.deltaTime);
        //Slerp calcula una orientacion intermedia entre la actual y la deseada, por eso vamos dondole datos, la rotacion que tiene ahora
        //el npc, la que queremos que tenga, y con el calculo de la velocidad haremos cuanto queremos que gire por cada fotograma, 
        //de esta manera no tendra un giro brusco sino uno mas progresivo entre fotogramas

        //Sumamos desde su posicion actual el desplazamiento que va a tener
        npc.transform.position += npc.transform.forward * velocidad * Time.deltaTime;
        //ira hacia delante segun su nueva posicion al girarse, y con la velocidad y delta se calcula la distancia que recorre por
        //cada fotograma, asi va progresivamente. Se suma a su posicion actual para que pueda esta cambiar
    }

    public override void Exit()
    {
        Debug.Log("NPC: dejo de patrullar.");
    }

    //metodos complementarios
}

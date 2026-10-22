
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcState
{
    // esta clase esta hecha para sentar una especie de base de lo que utilizad
    // cada transicion y stado. Despues cada npc debera implementar lo que necesite
    // a partir de esta, es como una especie de plantilla para los estado
    
    protected NpcController npc; //controlador del npc
    private Dictionary<string, Type> transitions =
        new(); //"lista" de transicones

    protected NpcState(NpcController npc)
    {
        this.npc = npc;
    }
    public virtual void Enter() { } //metodo para empezar estado
    public abstract void Execute(); //metodo para el update
    public virtual void Exit() { } // metodos para salir del estado
    protected void AddTransition(string eventName, Type nextState)
    {
        transitions[eventName] = nextState;
        // mira que estado empieza el evento que nos pasan
    }
    public Type GetNextState(string eventName)
    {
        if (transitions.TryGetValue(eventName, out Type nextState))
        {
            return nextState;
        }
        //busca la transicion
        return null;
    }
}
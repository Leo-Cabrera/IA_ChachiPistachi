
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcState
{
    // esta clase está hecha para sentar una especie de base de lo que utilizad
    // cada transición y stado. Después cada npc deberá implementar lo que necesite
    // a partir de esta, es como una especie de plantilla para los estado
    
    protected NpcController npc; //controlador del npc
    private Dictionary<string, Type> transitions =
        new(); //"lista" de transicones

    protected NpcState(NpcController npc)
    {
        this.npc = npc;
    }
    public virtual void Enter() { } //método para empezar estado
    public abstract void Execute(); //método para el update
    public virtual void Exit() { } // método para bsalir del estado
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
        //busca la transición
        return null;
    }
}
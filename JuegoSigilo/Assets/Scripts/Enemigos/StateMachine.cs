
using System;
using System.Collections.Generic;

public class StateMachine
{
    //La mquina de estado solo dice que pases de un estado a otro de los
    //tiene registrados para ejecutar, ya de lo que hagan esos estados
    //se ocupa cada npc por su cuenta.
    public NpcState CurrentState { get; private set; }
    public NpcState PreviousState { get; private set; }

    private Dictionary<Type, NpcState> states =
        new();
    //Registramos estado existente
    public void RegisterState(NpcState state)
    {
        states[state.GetType()] = state; 
    }

    //Metodo para cambiar de estado
    public void SetState(Type stateType)
    {
        // Revisamos que el estado esta registrado
        if (!states.TryGetValue(stateType, out NpcState newState))
        {
            throw new Exception(
                "Estado no encontrado: " + stateType.Name
            );
        }

        // Salimos del estado anterior
        CurrentState?.Exit();

        PreviousState = CurrentState;
        CurrentState = newState;

        // Inicializamos el nuevo estado
        CurrentState.Enter();
    }

    // Ejecutar estado actual
    public void Update()
    {
        CurrentState?.Execute();
    }

    // Metodo para procesar un evento y cambiar al estado que toque
    public void TriggerEvent(string eventName)
    {
        if (CurrentState == null)
            return;

        Type nextState = CurrentState.GetNextState(eventName);

        if (nextState != null)
        {
            SetState(nextState);
        }
    }
}

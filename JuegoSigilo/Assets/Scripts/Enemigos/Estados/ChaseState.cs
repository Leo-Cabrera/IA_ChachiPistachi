
using UnityEngine;
using System;

public class ChaseState : NpcState
{
    private float velocidad;
    private float rotacionVel;

    public ChaseState(
        NpcController npc,
        float velocidad,
        float rotacionVel) : base(npc)
    {
        this.velocidad = velocidad;
        this.rotacionVel = rotacionVel;

        if (npc is ArmorController)
        {
            AddTransition("PlayerLost", typeof(DefenseState));
            AddTransition("PlayerHidden", typeof(DefenseState));
        }
        else if (!(npc is MiniGhostController))
        {
            AddTransition("PlayerLost", typeof(PatrolState));
        }
    }

    public void ConfigurarTransicion(
        string evento, Type siguienteEstado)
    {
        AddTransition(evento, siguienteEstado);
    }

    public override void Enter()
    {
        npc.ReiniciarCamino();
        Debug.Log("NPC: comienzo a perseguir.");
    }

    public override void Execute()
    {
        if (npc is SpiritController spirit &&
            spirit.EsperandoTrasEscondite)
            return;

        if (npc is ArmorController armor &&
            armor.EsperandoTrasEscondite)
            return;

        if (npc.target == null)
            return;

        npc.ActualizarCamino(npc.target.position);
        npc.MoverPorCamino(velocidad, rotacionVel);
    }

    public override void Exit()
    {
        npc.ReiniciarCamino();
        Debug.Log(npc.gameObject.name + ": dejo de perseguir.");
    }
}

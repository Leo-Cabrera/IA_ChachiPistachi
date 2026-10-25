using UnityEngine;

public class DefenseState : NpcState
{
    private float vel;
    private float rotVel;
    private float defenseTime;
    private float defenseCrono;
    private ArmorController npc;

    
    public DefenseState(ArmorController npc, float vel, float rotVel, float defenseTime, float defenseCrono) : base(npc)
    {
        this.vel = vel;
        this.rotVel = rotVel;
        this.defenseTime = defenseTime;
        this.defenseCrono = defenseCrono;
        this.npc = npc;
    }
    public void ConfigurarTransicion(string eventName, System.Type nextState)
    {
        AddTransition(eventName, nextState);
    }

    public override void Enter()
    {
        npc.ReiniciarCamino();
        Debug.Log("NPC: Entrando en estado de defensa.");
        defenseCrono = 0f; 

    }

    public override void Execute()
    {
        if (!npc.withYaiko)
        {

            npc.ActualizarCamino(npc.yaikoPos.position);
            npc.MoverPorCamino(vel, rotVel);
            npc.armorAnimator.SetBool("Walk", true);
        }
        else
        {
            npc.armorAnimator.SetBool("Walk", false);
            defenseCrono += Time.deltaTime;
            if (defenseCrono >= defenseTime)
            {
                defenseCrono = 0f;
                npc.EndDefense();
            }
            
        }
    }
    public override void Exit()
    {
        npc.ReiniciarCamino();
        Debug.Log("NPC: Saliendo del estado de defensa.");
    }
}
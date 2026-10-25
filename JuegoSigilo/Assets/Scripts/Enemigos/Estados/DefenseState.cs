using UnityEngine;

public class DefenseState : NpcState
{
    private float vel;
    private float rotVel;
    private float defenseTime;
    private float defenseCrono;
    private ArmorController npc;
    private Nodo nodoDestinoDefensa;


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

        npc.ActualizarCamino(npc.yaikoPos.position);

        if (npc.Camino == null)
            Debug.Log("Armor: no se ha calculado un camino hacia Yaiko");
        else
            Debug.Log("Armor: camino hacia Yaiko de " + npc.Camino.Count + " nodos");

        Parilla parrilla = UnityEngine.Object.FindFirstObjectByType<Parilla>();
        Nodo nodoYaiko = parrilla.ObtenerNodoDesdePosicion(npc.yaikoPos.position);
        Debug.Log("Armor: nodo de Yaiko caminable = " + nodoYaiko.caminable);

        Nodo nodoArmor = parrilla.ObtenerNodoDesdePosicion(npc.transform.position);
        nodoDestinoDefensa = null;

        for (int radio = 1; radio <= 4 && nodoDestinoDefensa == null; radio++)
        {
            for (int x = -radio; x <= radio && nodoDestinoDefensa == null; x++)
            {
                for (int z = -radio; z <= radio; z++)
                {
                    // Revisamos solo el borde de cada anillo, sin repetir casillas interiores.
                    if (Mathf.Abs(x) != radio && Mathf.Abs(z) != radio)
                        continue;

                    Vector3 posicion = npc.yaikoPos.position
                        + new Vector3(x * parrilla.tamNodo, 0f, z * parrilla.tamNodo);

                    Nodo candidato = parrilla.ObtenerNodoDesdePosicion(posicion);

                    if (!candidato.caminable)
                        continue;

                    if (candidato == nodoArmor ||
                        parrilla.BuscarCamino(nodoArmor, candidato).Count > 0)
                    {
                        nodoDestinoDefensa = candidato;
                        break;
                    }
                }
            }
        }

        if (nodoDestinoDefensa == null)
            Debug.LogWarning("Armor: no encontró una casilla alcanzable cerca de Yaiko");
        else
            Debug.Log("Armor: casilla elegida cerca de Yaiko: " + nodoDestinoDefensa.posicionMundo);

    }

    public override void Execute()
    {
        if (nodoDestinoDefensa != null &&
        !npc.EstaEnNodoDe(nodoDestinoDefensa.posicionMundo))
        {
            npc.ActualizarCamino(nodoDestinoDefensa.posicionMundo);

            if (npc.Camino != null && npc.Camino.Count > 0)
            {
                npc.MoverPorCamino(vel, rotVel);
                npc.armorAnimator.SetBool("Walk", true);
                return;
            }
        }

        npc.armorAnimator.SetBool("Walk", false);
        defenseCrono += Time.deltaTime;

        if (defenseCrono >= defenseTime)
            npc.EndDefense();
    }
    public override void Exit()
    {
        npc.ReiniciarCamino();
        Debug.Log("NPC: Saliendo del estado de defensa.");
    }
}
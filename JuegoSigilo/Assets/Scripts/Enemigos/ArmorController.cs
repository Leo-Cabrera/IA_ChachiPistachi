using UnityEngine;

public class ArmorController : NpcController
{
    public float velocidad = 1f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float arriveDistance = .4f;
    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)
       
        //FSM.SetState(typeof(PatrolState));
    }
}

using UnityEngine;

public class ZombieController : NpcController
{
    public float velocidad = 3f;
    public float RotacionVel = 5f;
    public float tiempoPrediccion = 3f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float arriveDistance = .4f;
    protected override void ConfigureStates()
    {
        //FSM.RegisterState(new PatrolState(this, patrolDetectionRange)

        //registramos en la fsm el estado de patrulla teniendo en cuenta los nuevos parametros del constructor
        FSM.RegisterState(new PatrolState(this, velocidad, RotacionVel));

        //registramos ahora el estado de persecucion
        FSM.RegisterState(new ChaseState(this, velocidad, RotacionVel));

        //comienza con el estado de patrulla
        FSM.SetState(typeof(PatrolState));


    }
}

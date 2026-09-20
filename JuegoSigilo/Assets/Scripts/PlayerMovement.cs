using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    private Vector2 movementInput;

    private void Update()
    {
        // Obtenemos el input de WASD.
        movementInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            movementInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            movementInput.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            movementInput.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            movementInput.x -= 1f;

        // Evita que diagonal sea mas rapida.
        movementInput = Vector2.ClampMagnitude(movementInput, 1f);
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(
            movementInput.x,
            0f,
            movementInput.y
        );

        Vector3 velocity = direction * speed;

        // Conservamos la velocidad vertical.
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    private void Rotate()
    {
        // Si no nos estamos moviendo, no giramos.
        if (movementInput.sqrMagnitude < 0.01f)
            return;

        Vector3 direction = new Vector3(
            movementInput.x,
            0f,
            movementInput.y
        );

        // Direccion hacia la que queremos mirar.
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Rotacion suave.
        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }
}

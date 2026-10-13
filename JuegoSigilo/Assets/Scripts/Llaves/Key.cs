using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    public GameObject key;
    public Collider pickZone;
    public bool active = true;
    void Start()
    {
        if (active) EnableKey();
        else DisableKey();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //para comprobar si el collider del jugador chocha con la llave
    private void OnTriggerEnter(Collider other)
    {
        if (!active) //si la llave no está activa
            return;

        if (!other.CompareTag("Player")) //si es otro personaje que no sea el jugador
            return;

        if (!(other is CapsuleCollider)) //si es otro collider que no sea la CapsuleCollider del jugador
            return;

        PlayerKeys playerKeys = other.GetComponent<PlayerKeys>();  //busca el componente Playerkeys y lo que da lo guarda en la variable

        if (playerKeys != null && playerKeys.AddKey())  //si hay contado y en addkey se devuelve true (o sea que tenemos menos de 3 llaves)
            DisableKey();  //solo si se cumple, es decir que se habrá sumado la llave al contador, si el jugador tuviera ya 3 llaves
            //entonces aunque pasara el jugador por la llave esta no desaparecería porque no se cumple addkey
    }

    public void EnableKey()
    {
        if (key != null)
        {
            key.SetActive(true);  //para enseñar el modelo 3d

            if (pickZone != null)
                pickZone.enabled = true;  //activa el collider de la llave

            active = true;

        }

    }
    public void DisableKey()
    {
        if (key != null)
        {

            if (pickZone != null)
                pickZone.enabled = false;  //desactiva el collider de la llave

            key.SetActive(false);  //el modelo desaparece
            active = false;

        }

    }
}


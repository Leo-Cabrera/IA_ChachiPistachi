using UnityEngine;

public class PlayerKeys : MonoBehaviour
{
    [Header("Configuración de Llaves")]
    [Tooltip("Cantidad actual de llaves del jugador (0 a 3)")]
    public int currentKeys = 0;

    [Tooltip("Límite máximo de llaves")]
    public int maxKeys = 3;

    private float ultimoGolpe = -999f; //para evitar perder todas las llaves a la vez

    //metodo para recoger una llave del mapa

    public void AddKey()
    {
        if (currentKeys < maxKeys) //si ahora mismo tengo menos del máximo de llaves
        {
            currentKeys++; //se puede añadir una mas
            Debug.Log($"Llaves actuales: {currentKeys}"); //para ver en la consola cuantas llaves tenemos
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) //si el jugador colisiona con un elemento con el tag 'enemy'
        {
            if (Time.time >= ultimoGolpe + 1.5f) //si han pasado 1.5 segundos o mas de la ultima colision, cuenta como valida
            {
                ultimoGolpe = Time.time; //cambiamos la variable ultimoGolpe al tiempo actual
                LoseKeyOrGameOver(); //y llamamos al metodo para eliminar llaves o perder la partida
            }
        }
    }

    public void LoseKeyOrGameOver()
    {
        if (currentKeys > 0) //si el jugador tiene al menos una llave
        {
            currentKeys--; //se resta una al total de las que tiene
            Debug.Log($"Has perdido 1 llave. Te quedan: {currentKeys}"); //y se muestra en la consola cuantas quedan
        }

        else //si el jugador no tiene ninguna llave
        {
            GameOver(); //pierde la partida
        }
    }

    private void GameOver() //este sera el metodo para perder
    {
        Debug.Log("Game Over"); //se vera en la consola que has perdido

        UnityEditor.EditorApplication.isPlaying = false; //placeholder de momento para 'perder' el juego que te saca del modo de juego del editor
    }
}

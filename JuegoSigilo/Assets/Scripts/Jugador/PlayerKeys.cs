using UnityEngine;

public class PlayerKeys : MonoBehaviour
{
    [Header("Configuración de Llaves")]
    [Tooltip("Cantidad actual de llaves del jugador (0 a 3)")]
    [SerializeField] private int currentKeys = 0;

    [Tooltip("Límite máximo de llaves")]
    [SerializeField] private int maxKeys = 3;
    [SerializeField] private SpawnKey spawnKey; //para poder llamar al script de SpawnKeys

    private float ultimoGolpe = -999f; //para evitar perder todas las llaves a la vez, se inicia así para siempre superar ese tiempo

    // Método para recoger una llave del mapa
    public bool AddKey()
    {
        if (currentKeys < maxKeys) // si ahora mismo tengo menos del máximo de llaves
        {
            currentKeys++; // se puede añadir una más
            Debug.Log($"Llaves actuales: {currentKeys}"); // para ver en la consola cuántas llaves tenemos
            return true;
        }

        return false;
    }

    private void OnCollisionStay(Collision collision) //ponemos OnCollisionStay porque si usamos el de Enter y el enemigo se queda pegado
                                                      //al jugador, solo detecta el tiempo de 1.5 si nos separamos y volvemos a chocar
    {
        if (collision.gameObject.CompareTag("Enemy")) //si el jugador choca con un enemigo
        {
            if (Time.time >= ultimoGolpe + 1.5f) //la primera vez siempre entra porque está en -999f
                                                 //deben haber pasado al menos 1,5 segundos para volver a recibir un golpe  (inmunidad)
            {
                ultimoGolpe = Time.time; //guardamos cuándo ocurrió este golpe
                LoseKeyOrGameOver(); //restamos una llave o mostramos Game Over
            }
        }
    }

    public void LoseKeyOrGameOver()
    {
        if (currentKeys > 0) //si el jugador tiene al menos una llave
        {
            currentKeys--; //se resta una al total de las que tiene
            Debug.Log($"Has perdido 1 llave. Te quedan: {currentKeys}");

            if (spawnKey != null)
                spawnKey.SpawnRandomKey(); //llamamos al scrip para que una inactiva aparezca
        }
        else //si el jugador no tiene ninguna llave y ha sido atacado
        {
            GameOver();
        }
    }

    private void GameOver() //este será el método para perder
    {
        Debug.Log("Game Over"); //por ahora solo lo comprobamos en la consola
    }

}
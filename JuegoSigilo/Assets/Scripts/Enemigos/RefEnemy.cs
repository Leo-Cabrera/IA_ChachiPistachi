using UnityEngine;

public class RefEnemy : MonoBehaviour
{
    public bool coneVisibility;
    public bool areaVisibility;
    public Renderer targetRenderer;

    [SerializeField] private GameObject visualModel;  //el hijo completo del enemigo (para agrupar todas las piezas del prefab)

    private void Awake()  //al empezar la partida se llama 
    {
        changeVisibility();
    }

    public void changeVisibility()
    {
        bool visible = coneVisibility || areaVisibility;  //si está en las zonas donde podemos verlo dará true

        if (visualModel != null) //si hemos asignado un objeto al campo de visualmodel
            visualModel.SetActive(visible);  //que se active dependiendo de si visible es true o false
        else if (targetRenderer != null)
            targetRenderer.enabled = visible;  //se renderiza dependiendo de si visible es true o false
    }
}
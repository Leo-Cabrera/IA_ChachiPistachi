using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.EventSystems.EventTrigger;

public class Wardrove : MonoBehaviour
{
    public Transform wardrove;
    public Transform camTransform;
    public GameObject hideButton;
    public GameObject exitButton;
    public float ButtonVel = 180f;
    public TextMeshProUGUI timerText;

    public float hideTime = 10f;
    private float timer;
    private bool hiding = false;

    private bool inWardroveZone = false;

    void Start()
    {
        exitButton.SetActive(false);
        hideButton.SetActive(false);
        timer = hideTime;
        
    }

    private void Update()
    {
        if (hiding)
        {
            timer -= Time.deltaTime;
            timerText.text = "" + timer.ToString("f2")+ "s";

            if (timer < 0)
            {
                StopHiding();
            }
        }
        
    }

    void LateUpdate()
    {
        if (inWardroveZone && hideButton.activeSelf )
        {
            hideButton.SetActive(true);
            UpdateButtonRotationPosition(hideButton);
        }
        else hideButton.SetActive(false);
        if(hiding) UpdateButtonRotationPosition(exitButton);
    }

    private void UpdateButtonRotationPosition(GameObject button)
    {
        if (wardrove == null || camTransform == null)
            return;

        Vector3 vectWardrove = button.transform.position - wardrove.position;
        float distancia = vectWardrove.magnitude;

        if (distancia == 0f)
            return;

        // Dirección deseada: del wardrove hacia la cámara
        Vector3 direccionObjetivo = camTransform.position - wardrove.position;

        // Órbita alrededor del eje Y del wardrove
        direccionObjetivo = Vector3.ProjectOnPlane(
            direccionObjetivo, wardrove.up
        );

        if (direccionObjetivo.sqrMagnitude < 0.0001f)
            return;

        direccionObjetivo.Normalize();

        Vector3 direccionActual = Vector3.ProjectOnPlane(
            vectWardrove, wardrove.up
        ).normalized;

        // Gira alrededor del wardrove manteniendo el radio
        float angulo = Vector3.SignedAngle(
            direccionActual,
            direccionObjetivo,
            wardrove.up
        );

        float giro = Mathf.MoveTowards(
            0f, angulo, ButtonVel * Time.deltaTime
        );

        button.transform.RotateAround(
            wardrove.position, wardrove.up, giro
        );

       
        button.transform.LookAt(camTransform.position, wardrove.up);
        button.transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El personaje ha entrado");
            inWardroveZone = true;
            hideButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El personaje ha salido");
            inWardroveZone = false;
            hideButton.SetActive(false);
        }
    }

    public void Hide()
    {
        hiding = true;
        PlayerData.instance.Hide();
        exitButton.SetActive(true);
    }

    public void StopHiding()
    {
        hiding = false;
        timer = hideTime;
        PlayerData.instance.StopHiding();
        exitButton.SetActive(false);

    }
}

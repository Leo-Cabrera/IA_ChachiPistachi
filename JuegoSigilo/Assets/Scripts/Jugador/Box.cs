using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.EventSystems.EventTrigger;

public class Box : MonoBehaviour
{
    public Transform box;
    public Transform camTransform;
    public GameObject jumpButton;
    public GameObject downButton;
    public float ButtonVel = 180f;
    public TextMeshProUGUI timerText;
    public GameObject fakePlayer;

    public float hideTime = 10f;
    private float timer;
    private bool onBox = false;

    private bool inBoxZone = false;

    void Start()
    {
        downButton.SetActive(false);
        jumpButton.SetActive(false);
        timer = hideTime;
        fakePlayer.SetActive(false);

    }

    private void Update()
    {
        if (onBox)
        {
            timer -= Time.deltaTime;
            timerText.text = "" + timer.ToString("f2") + "s";

            if (timer < 0)
            {
                DownBox();
            }
        }

    }

    void LateUpdate()
    {
        if (inBoxZone && jumpButton.activeSelf)
        {
            jumpButton.SetActive(true);
            UpdateButtonRotationPosition(jumpButton);
        }
        else jumpButton.SetActive(false);
        if (onBox) UpdateButtonRotationPosition(downButton);
    }

    private void UpdateButtonRotationPosition(GameObject button)
    {
        if (box == null || camTransform == null)
            return;

        Vector3 vectbox = button.transform.position - box.position;
        float distancia = vectbox.magnitude;

        if (distancia == 0f)
            return;

        // Dirección deseada: del box hacia la cámara
        Vector3 direccionObjetivo = camTransform.position - box.position;

        // Órbita alrededor del eje Y del box
        direccionObjetivo = Vector3.ProjectOnPlane(
            direccionObjetivo, box.up
        );

        if (direccionObjetivo.sqrMagnitude < 0.0001f)
            return;

        direccionObjetivo.Normalize();

        Vector3 direccionActual = Vector3.ProjectOnPlane(
            vectbox, box.up
        ).normalized;

        // Gira alrededor del box manteniendo el radio
        float angulo = Vector3.SignedAngle(
            direccionActual,
            direccionObjetivo,
            box.up
        );

        float giro = Mathf.MoveTowards(
            0f, angulo, ButtonVel * Time.deltaTime
        );

        button.transform.RotateAround(
            box.position, box.up, giro
        );


        button.transform.LookAt(camTransform.position, box.up);
        button.transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El personaje ha entrado");
            inBoxZone = true;
            jumpButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El personaje ha salido");
            inBoxZone = false;
            jumpButton.SetActive(false);
        }
    }

    public void OnBox()
    {
        onBox = true;
        PlayerData.instance.OnBox();
        downButton.SetActive(true);
        fakePlayer.SetActive(true);
    }

    public void DownBox()
    {
        onBox = false;
        timer = hideTime;
        PlayerData.instance.DownBox();
        downButton.SetActive(false);
        fakePlayer.SetActive(false);

    }
}

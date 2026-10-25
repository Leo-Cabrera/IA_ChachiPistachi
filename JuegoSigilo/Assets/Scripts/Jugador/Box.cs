using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.EventSystems.EventTrigger;

public class Box : MonoBehaviour
{
    public Transform box;
    private Transform camTransform;
    private CameraManager cameraManager;
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
        cameraManager = FindFirstObjectByType<CameraManager>();

    }

    private void Update()
    {
        if (cameraManager == null) return;



        if (cameraManager.camaraCenital.activeSelf)
        {
            camTransform = cameraManager.camaraCenital.transform;
        }
        else
        {
            camTransform = cameraManager.camaraInclinada.transform;
        }

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

        // Direcci�n deseada: del box hacia la c�mara
        Vector3 direccionObjetivo = camTransform.position - box.position;

        // �rbita alrededor del eje Y del box
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
        downButton.SetActive(true);
        fakePlayer.SetActive(true);
        downButton.transform.position = jumpButton.transform.position;
        downButton.transform.rotation = jumpButton.transform.rotation;

        ZombieController[] zombis = FindObjectsByType<ZombieController>(FindObjectsSortMode.None);

        foreach (ZombieController zombie in zombis)
        {
            zombie.AvisarEscondite(box.position);
        }

        PlayerData.instance.OnBox();
    }

    public void DownBox()
    {
        onBox = false;
        timer = hideTime;
        PlayerData.instance.DownBox();
        downButton.SetActive(false);
        fakePlayer.SetActive(false);
        jumpButton.transform.position = downButton.transform.position;
        jumpButton.transform.rotation = downButton.transform.rotation;

    }
}

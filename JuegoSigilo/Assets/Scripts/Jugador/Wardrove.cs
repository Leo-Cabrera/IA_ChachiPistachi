
using UnityEngine;
using TMPro;

public class Wardrove : MonoBehaviour
{
    public Transform wardrove;

    private Transform camTransform;
    private CameraManager cameraManager;

    public GameObject hideButton;
    public GameObject exitButton;
    public GameObject textSeconds;

    public float ButtonVel = 180f;
    public TextMeshProUGUI timerText;
    public float hideTime = 10f;

    private float timer;
    private float coolDown = 5f;

    private bool hiding = false;
    private bool coolDownActive = false;
    private bool inWardroveZone = false;

    [SerializeField] private ArmorController armadura;
    [SerializeField] private SpiritController espiritu;
    [SerializeField] private ZombieController zombie;

    private void Start()
    {
        if (espiritu == null)
            espiritu = FindFirstObjectByType<SpiritController>();

        if (armadura == null)
            armadura = FindFirstObjectByType<ArmorController>();

        if (zombie == null)
            zombie = FindFirstObjectByType<ZombieController>();

        if (exitButton != null)
            exitButton.SetActive(false);

        if (hideButton != null)
            hideButton.SetActive(false);

        if (textSeconds != null)
            textSeconds.SetActive(false);

        timer = hideTime;
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    private void Update()
    {
        if (cameraManager != null)
        {
            if (cameraManager.camaraCenital != null &&
                cameraManager.camaraCenital.activeSelf)
            {
                camTransform =
                    cameraManager.camaraCenital.transform;
            }
            else if (cameraManager.camaraInclinada != null)
            {
                camTransform =
                    cameraManager.camaraInclinada.transform;
            }
        }

        if (hiding)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = timer.ToString("f2") + "s";

            if (timer <= 0f)
                StopHiding();
        }

        if (coolDownActive)
        {
            coolDown -= Time.deltaTime;

            if (coolDown <= 0f)
            {
                coolDownActive = false;
                coolDown = 5f;
            }
        }
    }

    private void LateUpdate()
    {
        if (hideButton != null)
        {
            hideButton.SetActive(inWardroveZone && !hiding);

            if (inWardroveZone && !hiding)
                UpdateButtonRotationPosition(hideButton);
        }

        if (hiding && exitButton != null)
            UpdateButtonRotationPosition(exitButton);
    }

    private void UpdateButtonRotationPosition(GameObject button)
    {
        if (wardrove == null || camTransform == null || button == null)
            return;

        Vector3 vectWardrove =
            button.transform.position - wardrove.position;

        if (vectWardrove.sqrMagnitude == 0f)
            return;

        Vector3 direccionObjetivo =
            camTransform.position - wardrove.position;

        direccionObjetivo =
            Vector3.ProjectOnPlane(
                direccionObjetivo, wardrove.up);

        if (direccionObjetivo.sqrMagnitude < 0.0001f)
            return;

        direccionObjetivo.Normalize();

        Vector3 direccionActual =
            Vector3.ProjectOnPlane(
                vectWardrove, wardrove.up).normalized;

        float angulo = Vector3.SignedAngle(
            direccionActual,
            direccionObjetivo,
            wardrove.up);

        float giro = Mathf.Clamp(
            angulo,
            -ButtonVel * Time.deltaTime,
            ButtonVel * Time.deltaTime);

        button.transform.RotateAround(
            wardrove.position, wardrove.up, giro);

        button.transform.LookAt(
            camTransform.position, wardrove.up);

        button.transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inWardroveZone = true;

            if (hideButton != null && !hiding)
                hideButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inWardroveZone = false;

            if (hideButton != null)
                hideButton.SetActive(false);
        }
    }

    public void Hide()
    {
        if (hiding || coolDownActive)
        {
            Debug.Log("Cooldown activo o jugador ya escondido.");
            return;
        }

        // Avisamos primero a los NPC, antes de desactivar al jugador.
        if (armadura != null)
            armadura.JugadorSeHaEscondido();

        if (espiritu != null)
            espiritu.JugadorSeHaEscondido();

        if (zombie != null)
            zombie.JugadorSeHaEscondido();

        hiding = true;
        timer = hideTime;

        if (PlayerData.instance != null)
            PlayerData.instance.Hide();

        if (exitButton != null)
        {
            exitButton.SetActive(true);

            if (hideButton != null)
            {
                exitButton.transform.position =
                    hideButton.transform.position;

                exitButton.transform.rotation =
                    hideButton.transform.rotation;
            }
        }

        if (textSeconds != null)
            textSeconds.SetActive(true);
    }

    public void StopHiding()
    {
        if (!hiding)
            return;

        hiding = false;
        timer = hideTime;

        if (PlayerData.instance != null)
            PlayerData.instance.StopHiding();

        if (exitButton != null)
            exitButton.SetActive(false);

        if (textSeconds != null)
            textSeconds.SetActive(false);

        if (hideButton != null && exitButton != null)
        {
            hideButton.transform.position =
                exitButton.transform.position;

            hideButton.transform.rotation =
                exitButton.transform.rotation;
        }

        coolDownActive = true;
        coolDown = 5f;
    }
}

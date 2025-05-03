using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivityX = 2f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float minPitch = -80f;

    [Header("References")]
    [SerializeField] private Camera playerCameraComponent;
    [SerializeField] private AudioListener playerAudioListener;

    private Rigidbody rb;

    // Valores internos para suavizado y ángulos
    private float yaw;   // rotación en Y del jugador
    private float pitch; // rotación en X de la cámara
    private Vector2 currentMouseVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCameraComponent == null) Debug.LogError("ERROR: Asigna la Cámara en el Inspector", this);
        if (playerAudioListener == null) Debug.LogError("ERROR: Asigna el Audio Listener en el Inspector", this);
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerCameraComponent.enabled = true;
            playerAudioListener.enabled = true;
            rb.isKinematic = false;

            // Inicializamos los ángulos con la rotación actual
            yaw = transform.eulerAngles.y;
            pitch = playerCameraComponent.transform.localEulerAngles.x;
        }
        else
        {
            playerCameraComponent.enabled = false;
            playerAudioListener.enabled = false;
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Leemos el movimiento del ratón
        Vector2 mouseDelta = new Vector2(
            Input.GetAxis("Mouse X") * mouseSensitivityX,
            Input.GetAxis("Mouse Y") * mouseSensitivityY
        );

        // Suavizamos si queremos (opcional)
        mouseDelta = Vector2.SmoothDamp(
            Vector2.zero,
            mouseDelta,
            ref currentMouseVelocity,
            rotationSmoothTime
        );

        // Actualizamos yaw/pitch
        yaw += mouseDelta.x;
        pitch -= mouseDelta.y; // invertimos Y para estilo FPS clásico

        // Limitamos el pitch
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Aplicamos rotaciones:
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCameraComponent.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D o flechas
        float v = Input.GetAxis("Vertical");   // W/S o flechas

        // Movimiento relativo a la rotación Y del jugador (que coincide con yaw)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDir = (right * h + forward * v).normalized;
        Vector3 moveVel = moveDir * moveSpeed;

        // Movemos el Rigidbody
        rb.MovePosition(rb.position + moveVel * Time.deltaTime);
    }
}

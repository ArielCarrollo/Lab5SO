using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime; 

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
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

    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;       
    [SerializeField] private Slider healthBarSlider;    

    private Rigidbody rb;
    private float yaw;
    private float pitch;
    private Vector2 currentMouseVelocity;

    public static Camera LocalPlayerCamera { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCameraComponent == null) Debug.LogError("ERROR: Asigna la Cámara en el Inspector", this);
        if (playerAudioListener == null) Debug.LogError("ERROR: Asigna el Audio Listener en el Inspector", this);
        if (projectilePrefab == null) Debug.LogError("ERROR: Asigna el Projectile Prefab en el Inspector", this);
        if (firePoint == null) Debug.LogError("ERROR: Asigna el Fire Point en el Inspector", this);
        if (healthBarSlider == null) Debug.LogError("ERROR: Asigna el Health Bar Slider en el Inspector", this);

        currentHealth = maxHealth; // Inicializar vida
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerCameraComponent.enabled = true;
            playerAudioListener.enabled = true;
            rb.isKinematic = false;
            yaw = transform.eulerAngles.y;
            pitch = playerCameraComponent.transform.localEulerAngles.x;
            LocalPlayerCamera = playerCameraComponent;
            if (playerCameraComponent.tag != "MainCamera")
            {
                Debug.LogWarning("La cámara del jugador (" + gameObject.name + ") no está etiquetada como 'MainCamera'.", this);
            }
        }
        else
        {
            playerCameraComponent.enabled = false;
            playerAudioListener.enabled = false;
            rb.isKinematic = true;
        }
        UpdateHealthBarUI();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HandleMouseLook();
        HandleMovement();
        HandleShooting(); 
    }

    private void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            CmdFire();
        }
    }

    void CmdFire()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Debug.Log($"Player {photonView.ViewID} disparando.");
        // Instanciamos el proyectil a través de la red
        GameObject projectileGO = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.ownerViewID = photonView.ViewID;
        }
    }

    [PunRPC]
    public void TakeDamage(int damageAmount, PhotonMessageInfo info)
    {
        Debug.Log($"[PlayerController RPC] TakeDamage RECIBIDO por Player {photonView.ViewID} ({gameObject.name}). Daño: {damageAmount}. Enviado por: ActorNumber {info.Sender?.ActorNumber}, NickName '{info.Sender?.NickName ?? "N/A"}'. Vida ANTES: {currentHealth}");

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBarUI();

        Debug.Log($"[PlayerController RPC] Vida DESPUÉS para Player {photonView.ViewID}: {currentHealth}");

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    void UpdateHealthBarUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void HandleDeath()
    {
        Debug.Log($"Player {photonView.ViewID} ha muerto.");
        if (photonView.IsMine)
        {

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            this.currentHealth = (int)stream.ReceiveNext();
            UpdateHealthBarUI();
        }
    }
    private void HandleMouseLook()
    {
        Vector2 mouseDelta = new Vector2(
            Input.GetAxis("Mouse X") * mouseSensitivityX,
            Input.GetAxis("Mouse Y") * mouseSensitivityY
        );

        mouseDelta = Vector2.SmoothDamp(
            Vector2.zero,
            mouseDelta,
            ref currentMouseVelocity,
            rotationSmoothTime
        );

        yaw += mouseDelta.x;
        pitch -= mouseDelta.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCameraComponent.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDir = (right * h + forward * v).normalized;
        Vector3 moveVel = moveDir * moveSpeed;

        rb.MovePosition(rb.position + moveVel * Time.deltaTime);
    }
}

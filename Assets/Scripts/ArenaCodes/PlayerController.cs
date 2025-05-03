using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("References")]

    [SerializeField] private Camera playerCameraComponent; 
    [SerializeField] private AudioListener playerAudioListener; 

    private Rigidbody rb;
    private Camera localPlayerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerCameraComponent == null)
            Debug.LogError("ERROR: ¡Asigna la Cámara hija en el Inspector de PlayerController!", this);
        if (playerAudioListener == null)
            Debug.LogError("ERROR: ¡Asigna el Audio Listener hijo en el Inspector de PlayerController!", this);
    }

    private void Start()
    {
        if (photonView.IsMine) 
        {
            Debug.Log("Configurando jugador LOCAL.");
            if (playerCameraComponent != null)
            {
                playerCameraComponent.enabled = true;
                localPlayerCamera = playerCameraComponent; 
            }
            if (playerAudioListener != null) playerAudioListener.enabled = true;

            if (rb != null) rb.isKinematic = false;

        }
        else
        {
            Debug.Log("Configurando jugador REMOTO.");
            if (playerCameraComponent != null) playerCameraComponent.enabled = false;
            if (playerAudioListener != null) playerAudioListener.enabled = false;

            if (rb != null) rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            HandleRotation();
        }
    }

    private void HandleMovement()
    {
        if (!photonView.IsMine || rb == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveVelocity = direction * moveSpeed;

        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!photonView.IsMine || localPlayerCamera == null) return;

        Ray ray = localPlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; 

            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
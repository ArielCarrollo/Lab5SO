using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _cameraToLookAt;

    void Start()
    {
        // Intentar asignar la c�mara al inicio.
        // Se prioriza PlayerController.LocalPlayerCamera.
        // Camera.main es un respaldo.
        if (PlayerController.LocalPlayerCamera != null)
        {
            _cameraToLookAt = PlayerController.LocalPlayerCamera;
        }
        else if (Camera.main != null)
        {
            _cameraToLookAt = Camera.main;
            Debug.LogWarning("BillboardUI (" + gameObject.name + ") est� usando Camera.main como respaldo. Aseg�rate de que PlayerController.LocalPlayerCamera est� configurada por el jugador local.", this);
        }
        else
        {
            Debug.LogError("BillboardUI (" + gameObject.name + "): No se encontr� ninguna c�mara (ni PlayerController.LocalPlayerCamera ni Camera.main). El Billboard no funcionar�.", this);
            enabled = false; // Deshabilitar el script si no hay c�mara
        }
    }

    void LateUpdate()
    {
        // Si la c�mara no estaba disponible en Start, o por si cambia (aunque es raro para una est�tica bien manejada).
        if (_cameraToLookAt == null)
        {
            if (PlayerController.LocalPlayerCamera != null)
            {
                _cameraToLookAt = PlayerController.LocalPlayerCamera;
            }
            else if (Camera.main != null) // Intenta de nuevo con Camera.main
            {
                _cameraToLookAt = Camera.main;
            }
        }

        if (_cameraToLookAt != null)
        {
            // Opci�n 1: Billboard completo (el UI mira directamente a la c�mara en todos los ejes)
            // Esto es generalmente lo preferido para que el UI siempre sea legible.
            transform.LookAt(transform.position + _cameraToLookAt.transform.rotation * Vector3.forward,
                             _cameraToLookAt.transform.rotation * Vector3.up);

            // Opci�n 2: Billboard solo en el eje Y (el UI se mantiene vertical respecto al mundo)
            // Descomenta estas l�neas y comenta la Opci�n 1 si prefieres este comportamiento.
            // Vector3 directionToCamera = _cameraToLookAt.transform.position - transform.position;
            // directionToCamera.y = 0; // Proyectar sobre el plano XZ para ignorar la inclinaci�n vertical
            // if (directionToCamera.sqrMagnitude > 0.001f) // Evitar error si la direcci�n es cero
            // {
            //     // Queremos que el "frente" del Canvas (usualmente su eje Z local positivo) mire hacia la c�mara.
            //     // Si tu UI est� "plano" en el Canvas, LookRotation(directionToCamera) deber�a funcionar.
            //     Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            //     transform.rotation = targetRotation;
            // }
        }
    }
}

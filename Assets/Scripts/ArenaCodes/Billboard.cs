using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _cameraToLookAt;

    void Start()
    {
        // Intentar asignar la cámara al inicio.
        // Se prioriza PlayerController.LocalPlayerCamera.
        // Camera.main es un respaldo.
        if (PlayerController.LocalPlayerCamera != null)
        {
            _cameraToLookAt = PlayerController.LocalPlayerCamera;
        }
        else if (Camera.main != null)
        {
            _cameraToLookAt = Camera.main;
            Debug.LogWarning("BillboardUI (" + gameObject.name + ") está usando Camera.main como respaldo. Asegúrate de que PlayerController.LocalPlayerCamera esté configurada por el jugador local.", this);
        }
        else
        {
            Debug.LogError("BillboardUI (" + gameObject.name + "): No se encontró ninguna cámara (ni PlayerController.LocalPlayerCamera ni Camera.main). El Billboard no funcionará.", this);
            enabled = false; // Deshabilitar el script si no hay cámara
        }
    }

    void LateUpdate()
    {
        // Si la cámara no estaba disponible en Start, o por si cambia (aunque es raro para una estática bien manejada).
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
            // Opción 1: Billboard completo (el UI mira directamente a la cámara en todos los ejes)
            // Esto es generalmente lo preferido para que el UI siempre sea legible.
            transform.LookAt(transform.position + _cameraToLookAt.transform.rotation * Vector3.forward,
                             _cameraToLookAt.transform.rotation * Vector3.up);

            // Opción 2: Billboard solo en el eje Y (el UI se mantiene vertical respecto al mundo)
            // Descomenta estas líneas y comenta la Opción 1 si prefieres este comportamiento.
            // Vector3 directionToCamera = _cameraToLookAt.transform.position - transform.position;
            // directionToCamera.y = 0; // Proyectar sobre el plano XZ para ignorar la inclinación vertical
            // if (directionToCamera.sqrMagnitude > 0.001f) // Evitar error si la dirección es cero
            // {
            //     // Queremos que el "frente" del Canvas (usualmente su eje Z local positivo) mire hacia la cámara.
            //     // Si tu UI está "plano" en el Canvas, LookRotation(directionToCamera) debería funcionar.
            //     Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            //     transform.rotation = targetRotation;
            // }
        }
    }
}

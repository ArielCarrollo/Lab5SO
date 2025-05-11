using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifetime = 3f;
    public int ownerViewID;

    private Rigidbody rb;
    private PhotonView photonView;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        // Mover el proyectil hacia adelante
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        if (photonView != null && photonView.IsMine)
        {
            Invoke(nameof(DestroyProjectile), lifetime);
        }
        else if (photonView == null) // Si no es un objeto de red, destruir localmente
        {
            Destroy(gameObject, lifetime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Log inicial para saber que OnTriggerEnter se disparó y con qué.
        Debug.Log($"[Projectile] OnTriggerEnter con: {other.gameObject.name} (Tag: {other.tag}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}). Proyectil del owner: {ownerViewID}. ¿Este proyectil es mío (IsMine)?: {photonView?.IsMine}");

        if (photonView != null && !photonView.IsMine)
        {
            return;
        }
        Debug.Log($"[Projectile] Este proyectil ES EL DUEÑO y procesará el impacto.");

        PlayerController hitPlayer = other.GetComponentInParent<PlayerController>(); 

        if (hitPlayer != null)
        {
            Debug.Log($"[Projectile] PlayerController encontrado en '{hitPlayer.gameObject.name}' (ViewID: {hitPlayer.photonView.ViewID}). Dueño del proyectil: {ownerViewID}.");

            // Evitar auto-daño
            if (hitPlayer.photonView.ViewID == ownerViewID)
            {
                Debug.Log($"[Projectile] Impacto en uno mismo (Player ViewID {ownerViewID}). No se aplica daño.");
                return;
            }

            Debug.Log($"[Projectile] Intentando enviar RPC 'TakeDamage' al Player ViewID: {hitPlayer.photonView.ViewID} con daño: {damage}");
            hitPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage); 

            if (other.GetComponent<Projectile>() == null)
            {
                DestroyProjectile();
            }
        }
        else
        {
            Debug.LogWarning($"[Projectile] Impacto con '{other.gameObject.name}', pero NO se encontró PlayerController en él o sus padres.");

            if (other.GetComponent<Projectile>() == null)
            {
            }
        }
    }

    void DestroyProjectile()
    {
        if (photonView != null && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else if (photonView == null && gameObject != null) 
        {
            Destroy(gameObject);
        }
    }
}
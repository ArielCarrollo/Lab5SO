using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Ejemplo: Reducir vida al recibir da�o (solo el due�o puede modificarla)
    public void TakeDamage(float damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    // Sincronizaci�n de la vida
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();
            UpdateHealthBar(); // Actualizar UI
        }
    }

    private void UpdateHealthBar()
    {
        // Aseg�rate de que el Slider est� referenciado
        Slider healthSlider = GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string arenaSceneName = "ArenaScene";

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom llamado en GameManager");

        // Solo el Master Client carga la escena para todos
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Soy el Master Client, cargando la arena...");
            PhotonNetwork.LoadLevel(arenaSceneName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Jugador {newPlayer.NickName} ha entrado a la sala");
    }
}
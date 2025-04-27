using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnectionTest : MonoBehaviourPunCallbacks
{
    private bool _waitingForNickname = true;

    public void ConnectToPhoton()
    {
        Debug.Log("Iniciando conexión a Photon...");
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;

        // Añadir el índice del personaje a las propiedades del jugador
        var playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps.Add("characterIndex", MasterManager.GameSettings.SelectedCharacterIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon.");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado al lobby de Photon.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Desconectado de Photon. Motivo: {cause}");
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class PhotonConnectionTest : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Iniciando conexión a Photon...");
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("? Conectado al servidor de Photon.");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        if(!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("?? Conectado al lobby de Photon.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"? Desconectado de Photon. Motivo: {cause}");
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerListingsMenu _playerListingsMenu;

    [SerializeField]
    private LeaveRoomMenu _leaveRoomMenu;
    private RoomsCanvases _roomsCanvases;
    [SerializeField] private Button startGameButton;

    // Reference
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
        _playerListingsMenu.FirstInitialize(canvases);
        _leaveRoomMenu.FirstInitialize(canvases);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public override void OnMasterClientSwitched(Player newMaster)
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
}

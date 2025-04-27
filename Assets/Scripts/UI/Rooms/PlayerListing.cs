using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite[] characterSprites;

    public Player Player { get; private set; }

    public void SetPlayerInfo(Player player)
    {
        Player = player;
        _text.text = player.NickName;
        // Obtener el índice del personaje desde las propiedades del jugador
        if (player.CustomProperties.TryGetValue("characterIndex", out object index))
        {
            int charIndex = (int)index;
            characterImage.sprite = characterSprites[charIndex];
        }
    }
}
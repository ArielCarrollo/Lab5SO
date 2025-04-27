using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NicknameCanvas : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject mainCanvases;
    [SerializeField] private CharacterSelector characterSelector;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmNickname);
        mainCanvases.SetActive(false);
    }

    private void OnConfirmNickname()
    {
        if (!string.IsNullOrEmpty(nicknameInputField.text))
        {
            // Guardar nickname y personaje seleccionado
            MasterManager.GameSettings.SetNickname(nicknameInputField.text);
            MasterManager.GameSettings.SetCharacterIndex(characterSelector.GetSelectedCharacterIndex());

            mainCanvases.SetActive(true);
            gameObject.SetActive(false);
            FindObjectOfType<PhotonConnectionTest>().ConnectToPhoton();
        }
    }
}

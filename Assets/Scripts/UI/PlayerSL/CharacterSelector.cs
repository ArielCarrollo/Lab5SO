using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private Image characterDisplay;
    [SerializeField] private Sprite[] characterSprites; // Todas las imágenes de personajes
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private int currentIndex = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextCharacter);
        prevButton.onClick.AddListener(PreviousCharacter);
        UpdateCharacterDisplay();
    }

    private void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterSprites.Length;
        UpdateCharacterDisplay();
    }

    private void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characterSprites.Length) % characterSprites.Length;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        characterDisplay.sprite = characterSprites[currentIndex];
    }

    public int GetSelectedCharacterIndex() => currentIndex;
}

using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private string _gameVersion = "0.0.0";
    public string GameVersion => _gameVersion;

    [SerializeField] private string _nickName = "Punfish";
    private string _customNickName = "";
    private int _selectedCharacterIndex = 0; // Índice del personaje seleccionado

    public string NickName
    {
        get
        {
            if (!string.IsNullOrEmpty(_customNickName)) return _customNickName;
            int value = Random.Range(0, 9999);
            return _nickName + value.ToString();
        }
    }

    public int SelectedCharacterIndex => _selectedCharacterIndex;

    public void SetNickname(string newNickname) => _customNickName = newNickname;
    public void SetCharacterIndex(int index) => _selectedCharacterIndex = index;
}
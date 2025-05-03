using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    [SerializeField] private string arenaSceneName = "ArenaScene";

    private const string GAME_STARTED_PROP = "gameStarted";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoadedInternal;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoadedInternal;
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[GameManager] Master Client iniciando el juego...");

            Hashtable props = new Hashtable
            {
                { GAME_STARTED_PROP, true }
                // { "sceneToLoad", arenaSceneName }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(arenaSceneName);
        }
    }

    // Este callback se ejecuta en TODOS los clientes cuando las propiedades de la sala cambian
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("[GameManager] Propiedades de la sala actualizadas: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());

        // Verifica si la propiedad que cambió fue la de inicio de juego
        if (propertiesThatChanged.ContainsKey(GAME_STARTED_PROP))
        {
            bool gameHasStarted = (bool)propertiesThatChanged[GAME_STARTED_PROP];

            if (gameHasStarted && !PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != arenaSceneName)
            {
                Debug.Log("[GameManager] Cliente detectó inicio de juego vía propiedades. Cargando escena: " + arenaSceneName);
                PhotonNetwork.LoadLevel(arenaSceneName);
            }
        }
    }

    // Usamos el callback de Unity en lugar de OnLevelWasLoaded para mayor fiabilidad con DontDestroyOnLoad
    void OnSceneLoadedInternal(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == arenaSceneName)
        {
            if (!PhotonNetwork.InRoom)
            {
                Debug.LogWarning("[GameManager] Escena Arena cargada, pero no estamos en una sala de Photon.");
                return;
            }

            bool playerAlreadyInstantiated = false;
            PlayerController[] existingPlayers = FindObjectsOfType<PlayerController>();
            foreach (var player in existingPlayers)
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    playerAlreadyInstantiated = true;
                    break;
                }
            }

            if (!playerAlreadyInstantiated)
            {
                Debug.Log($"[GameManager] Escena '{arenaSceneName}' cargada (OnSceneLoadedInternal). Instanciando PlayerPrefab...");
                PhotonNetwork.Instantiate("PhotonPrefabs/PlayerPrefab", Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"[GameManager] Escena '{arenaSceneName}' cargada, pero PlayerPrefab para este cliente ya existe.");
            }
        }
    }
}
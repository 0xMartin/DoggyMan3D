using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{

    /***********************************************************************************************************************************/
    // GAME CONFIG
    /***********************************************************************************************************************************/

    [Header("Player")]
    public GameObject PlayerRef;

    [Header("Main Camera")]
    public GameObject MainCamera;

    [Header("Levels")]
    [Tooltip("Seznam vsech levelu. Zadava se ID sceny, ve ktere se nachazi level hry. Tato nacitana scena musi obsahovat jednu komponentu Level.")]
    public int[] LevelsID;
    [Tooltip("Cas jak dloho se bude trvat nez se level plne spusti (postupne odtmaveni obrazovky). Pocitano po nacteni levelu.")]
    public float TimeToShowLevel = 2.0f;
    [Tooltip("Cas jak dloho se bude trvat nez se level plne ukonci (postupne ztmaveni obrazovky).")]
    public float TimeToHideLevel = 2.0f;
    [Tooltip("Nazev objektu ve scene levelu, ktery bude obsahovat deskriptor Levelu")]
    public string LevelInfoObjName = "LevelInfoObject";
    [Tooltip("ID sceny prvniho levelu ve hre. Vsechny sceny s levely musi mit indexy na konci")]
    public int FirstLevelSceneID = 2;

    [Header("Game Pause Menu")]
    public GameObject PauseMenu;
    public Button PauseMenuResume;
    public Button PauseMenuReset;
    public Button PauseMenuBackToMenu;

    [Header("Game Death Menu")]
    public GameObject DeathMenu;
    public Button DeathMenuContinue;
    public Button DeathMenuBackToMenu;

    [Header("Player UI")]
    public GameObject PlayerUI;

    [Header("Game Input System")]
    public GameInputs GameInputSystem;

    /***********************************************************************************************************************************/
    // GLOBAL VARIABLES SECTION
    /***********************************************************************************************************************************/

    private static PlayerSave _playerSave = null;
    private static Level _currentLevel = null;
    private static int _currentLevelId = 0;
    private static GameObject _camera = null;
    private static string _playerSaveToLoadPath = null;
    private static string _playerNameToSet = null;
    private static bool _escWasPressedLastTime = false;
    private static bool _gamePauseMenuIsVisible = false;
    private static bool _gameDeathMenuIsVisible = false;

    public static PlayerSave GetPlayerSave()
    {
        return _playerSave;
    }

    public static Level GetCurrentLevel()
    {
        return _currentLevel;
    }

    public static int GetCurrentLevelSceneId()
    {
        return _currentLevelId;
    }

    public static GameObject GetMainCamera()
    {
        return _camera;
    }

    public static void SetPlayerSaveToLoadOnGameManagerStart(string savePath)
    {
        _playerSaveToLoadPath = savePath;
    }

    public static void SetPlayerNameOnGameManagerStart(string name)
    {
        _playerNameToSet = name;
    }

    public static bool IsGamePaused()
    {
        return _gamePauseMenuIsVisible || _gameDeathMenuIsVisible;
    }

    /***********************************************************************************************************************************/
    // MAIN CONTROLL FUNCTIONS
    //
    // Actions:
    // On Awake (open game) = create new game / load game -> load level
    // On Level Finished = save game -> next level -> remove current level -> load level
    // On Level Close = Go to menu
    // On Level Reset = remove current level -> load level
    // On Player Death = Show menu
    //      -> Menu Action: Player Again = On Reset
    //      -> Menu Action: To Menu = On Level Cloese
    /***********************************************************************************************************************************/

    private void Start()
    {
        // dialog setup (Death menu)
        DeathMenuBackToMenu.onClick.AddListener(OnLevelClose);
        DeathMenuContinue.onClick.AddListener(OnLevelReset);

        // dialog setup (Pause menu)
        PauseMenuBackToMenu.onClick.AddListener(OnLevelClose);
        PauseMenuReset.onClick.AddListener(OnLevelReset);
        PauseMenuResume.onClick.AddListener(Resume);
    }

    // On Awake (open game) 
    private void Awake()
    {
        // dialogs setup
        GameInputSystem.SetCursorState(true);
        _gamePauseMenuIsVisible = false;
        _gameDeathMenuIsVisible = false;
        PauseMenu.SetActive(false);
        DeathMenu.SetActive(false);
        PlayerUI.SetActive(true);

        // game setup
        _camera = MainCamera;

        if (PlayerRef != null)
        {
            PlayerRef.SetActive(false);
        }

        // nacteni/vytvoreni save
        if (_playerSaveToLoadPath == null)
        {
            // create new game/save
            CreateNewSave();
        }
        else
        {
            // load existing game
            LoadPlayerSave();
            // rename player in existing save ???
            if (_playerNameToSet != null)
            {
                _playerSave.Name = _playerNameToSet;
                PlayerRef.GetComponent<GameEntityObject>().Name = _playerSave.Name;
            }
        }

        // nacte pozadovany aktualni level
        LoadLevel(_playerSave.Level);
    }

    private void Update()
    {
        if (GameInputSystem != null)
        {
            if (GameInputSystem.esc && !_escWasPressedLastTime)
            {
                // zobrazi/skryje game pause menu
                _gamePauseMenuIsVisible = !_gamePauseMenuIsVisible;
                PauseMenu.SetActive(_gamePauseMenuIsVisible);
                PlayerUI.SetActive(!_gamePauseMenuIsVisible);
                GameInputSystem.SetCursorState(!_gamePauseMenuIsVisible);
                Time.timeScale = IsGamePaused() ? 0.0f : 1.0f;
            }
            _escWasPressedLastTime = GameInputSystem.esc;
        }
    }

    private void Resume()
    {
        // skryje game pause menu
        _gamePauseMenuIsVisible = false;
        PauseMenu.SetActive(_gamePauseMenuIsVisible);
        PlayerUI.SetActive(!_gamePauseMenuIsVisible);
        GameInputSystem.SetCursorState(!_gamePauseMenuIsVisible);
        Time.timeScale = IsGamePaused() ? 0.0f : 1.0f;
    }

    // On Level Finished
    public void OnLevelFinished()
    {
        Debug.Log("Action - Level Finished");

        // jit do dalsiho levelu a ulozit hru
        GoToNextLevel();

        // reload levelu (aktualni odstrani a nacte dalsi level)
        ReloadCurrentLevel(_playerSave.Level);
    }

    // On Level Close
    public void OnLevelClose()
    {
        Debug.Log("Action - Level Closed");
        DeathMenu.SetActive(false);
        PlayerUI.SetActive(false);
    }

    // On Level Reset
    public void OnLevelReset()
    {
        Debug.Log("Action - Level Reset");
        DeathMenu.SetActive(false);
        PlayerUI.SetActive(false);

        // reload levelu (aktualni odstrani a nacte ten stejny level)
        ReloadCurrentLevel(_playerSave.Level);
    }

    // On Player Death
    public void OnPlayerDeath()
    {
        Debug.Log("Action - Player Death");

        // zobrazi game death menu
        DeathMenu.SetActive(true);
        PlayerUI.SetActive(false);
    }

    /***********************************************************************************************************************************/
    // GAME MANAGER SCENE LOAD FUNCTIONS
    /***********************************************************************************************************************************/

    /// <summary>
    /// Nacte save/hru ze souboru. Pokud se nacteni nepodari je automaticky vytvoren a nastaven novy save.
    /// </summary>
    private void LoadPlayerSave()
    {
        Debug.Log("Loading player save: " + _playerSaveToLoadPath);
        _playerSave = SaveSystem.LoadPlayer(_playerSaveToLoadPath);
        if (_playerSave != null)
        {
            GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
            _playerSave.PlayerRef = entity;
            entity.Name = _playerSave.Name;
            Debug.LogError("Save loading done");
        }
        else
        {
            Debug.LogError("Failed to load player save");
            CreateNewSave();
        }
    }

    /// <summary>
    /// Nacte vybrany level, spawne hrace a zahaji hru.
    /// </summary>
    /// <param name="level">Level ktery bude nacten</param>
    private void LoadLevel(int level)
    {
        if (level < LevelsID.Count())
        {
            Debug.Log("Start loading level: " + level);
            StartCoroutine(LoadLevelAsync(LevelsID[level]));
        }
        else
        {
            Debug.LogError("Failed to load level: " + level);
        }
    }

    private IEnumerator LoadLevelAsync(int levelId)
    {
        if (levelId >= FirstLevelSceneID)
        {
            // skryje player UI
            PlayerUI.SetActive(false);

            // reset hrace
            ResetPlayer();

            // zahaji nacitani levelu
            Debug.Log("Start loading scene with ID: " + levelId);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelId, LoadSceneMode.Additive);
            // pocka neze je nacitani sceny/levelu dokonceno
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // najde LevelInfo objekt a ziska Level deskriptor
            GameObject levelInfo = GameObject.Find(LevelInfoObjName);
            if (levelInfo != null)
            {
                _currentLevel = levelInfo.GetComponent<Level>();
                if (_currentLevel != null)
                {
                    // nastaveni id aktualne nactene sceny
                    _currentLevelId = levelId;

                    // pripravi hrace
                    SpawnPlayer();
                    ActivatePlayer();

                    // zobrazi player UI
                    PlayerUI.SetActive(true);

                    // bindovani eventu levelu
                    BindLevelEvents();

                    Debug.Log("Level loading done");
                }
                else
                {
                    Debug.LogError("Failed to get Level descriptor");
                }
            }
            else
            {
                Debug.LogError("LevelInfo not found");
            }
        }
        else
        {
            Debug.LogError("Invalid scene ID");
        }
    }

    /// <summary>
    /// Odstrani aktualni level, ktery je nacten ve scene. A pote nacte pozadovany level. Pokud aktualne neni nacteny zadny 
    /// level ve tak se automaticky vola metoda pro nacteni levelu
    /// </summary>
    /// <param name="level">Level ktery bude nacten</param>
    private void ReloadCurrentLevel(int level)
    {
        // overeni existence levelu, ktery ma byt nacten
        if (level >= LevelsID.Count())
        {
            Debug.LogError("Level with ID (" + level + ") not exists");
        }

        // deaktivace hrace
        DeactivePlayer();
        if (PlayerRef != null)
        {
            PlayerRef.SetActive(false);
        }

        // reset hrace
        ResetPlayer();

        // spusti reload sceny
        Scene sceneToUnload = SceneManager.GetSceneByBuildIndex(_currentLevelId);
        if (sceneToUnload == null)
        {
            // aktualne neni nactena zadna scena/level ve hre => zavola metoda pro nacteni levelu
            Debug.Log("No scene to unload! Calling load level function ...");
            LoadLevel(level);
        }
        else
        {
            // spusti reaload sceny (prvni odstani aktlualne nactenou scenu ve hre a pak nacte pozadovanou scenu/level)
            Debug.Log("Start realoading scene");
            if (sceneToUnload.isLoaded)
            {
                StartCoroutine(ReloadSceneAsync(sceneToUnload, LevelsID[level]));
            }
        }
    }

    private IEnumerator ReloadSceneAsync(Scene sceneToRemove, int leveToLoadId)
    {
        // skryje player UI
        PlayerUI.SetActive(false);

        // odstraneni aktualne nacteneho levelu
        Debug.Log("Start with unloading current scene wit ID: " + _currentLevelId);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToRemove);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        Debug.Log("Current level unloaded");

        // nacteni noveho levelu
        yield return StartCoroutine(LoadLevelAsync(leveToLoadId));

        Debug.Log("Scene reload done");
    }

    /***********************************************************************************************************************************/
    // GAME MANAGER UTILS
    /***********************************************************************************************************************************/

    /// <summary>
    /// Vytvori novy save/hru
    /// </summary>
    private void CreateNewSave()
    {
        GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
        _playerSave = new PlayerSave
        {
            Name = _playerNameToSet == null ? "Doggy Man" : _playerNameToSet,
            Level = 0,
            PlayerRef = entity
        };
        entity.Name = _playerSave.Name;
        SaveSystem.SavePlayer(_playerSave);
        Debug.Log("New save created");
    }

    /// <summary>
    /// Prejde do dalsiho levelu a ulozi hru. Nenacita vsak level, jen zmeni level index.
    /// </summary>
    private void GoToNextLevel()
    {
        if (_playerSave != null)
        {
            _playerSave.Level++;
            SaveSystem.SavePlayer(_playerSave);
            Debug.Log("Go to next level: " + _playerSave.Level);
        }
        else
        {
            Debug.LogError("Failed to go to the next level");
        }
    }

    /// <summary>
    /// Resetuje hrace
    /// </summary>
    private void ResetPlayer()
    {
        if (PlayerRef != null)
        {
            GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
            if (entity != null)
            {
                entity.ResetPlayer();
                Debug.Log("Player reset done");
            }
            else
            {
                Debug.LogError("Failed to reset player");
            }
        }
        else
        {
            Debug.LogError("Failed to reset player");
        }
    }

    /// <summary>
    /// Aktivuje hrace. Aktivovany hrace muze provadet vsechny akce.
    /// </summary>
    private void ActivatePlayer()
    {
        if (PlayerRef != null)
        {
            GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
            if (entity != null)
            {
                entity.IsEntityEnabled = true;
                Debug.Log("Player activation done");
            }
            else
            {
                Debug.LogError("Failed to active player");
            }
        }
        else
        {
            Debug.LogError("Failed to active player");
        }
    }

    /// <summary>
    /// Deaktivuje hrace. Deaktivovany hrac nemuze delat nic, nachazi se ve scene, ale neni mozne jej ovladat, nedostava zasahy, nautoci, ...
    /// </summary>
    private void DeactivePlayer()
    {
        if (PlayerRef != null)
        {
            GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
            if (entity != null)
            {
                entity.IsEntityEnabled = false;
                Debug.Log("Player deactivation done");
            }
            else
            {
                Debug.LogError("Failed to deactive player");
            }
        }
        else
        {
            Debug.LogError("Failed to deactive player");
        }
    }

    /// <summary>
    /// Spawne hrace na spawnpoint v aktualne nactenem levelu.
    /// </summary>
    private void SpawnPlayer()
    {
        if (PlayerRef == null || _currentLevel == null)
        {
            Debug.LogError("Failed to spawn player. Player of level descriptor is null");
            return;
        }
        if (_currentLevel.SpawnPoint == null)
        {
            Debug.LogError("Failed to spawn player. Spawnpoint of current level is null");
            return;
        }

        // nastavi hraci pozici a rotaci stejnou jako spawnpointu
        PlayerRef.transform.position = _currentLevel.SpawnPoint.transform.position;
        Quaternion sourceRotation = _currentLevel.SpawnPoint.transform.rotation;
        PlayerRef.transform.rotation = Quaternion.Euler(0, sourceRotation.eulerAngles.y, 0);

        // aktivuje objekt hrace
        if (PlayerRef != null)
        {
            PlayerRef.SetActive(true);
        }

        Debug.Log("Player spawned");
    }

    private void BindLevelEvents()
    {
        if (_currentLevel != null)
        {
            // level finished event
            _currentLevel.OnLevelFinished += this.OnLevelFinished;

            // player death event
            GameEntityObject entity = PlayerRef.GetComponent<GameEntityObject>();
            if (entity != null)
            {
                entity.OnDeath += this.OnPlayerDeath;
            }

            Debug.Log("Level events binding done");
        }
        else
        {
            Debug.LogError("Failed to bind level event. Current level is null");
        }
    }

}

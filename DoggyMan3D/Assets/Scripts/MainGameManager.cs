using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    /***********************************************************************************************************************************/
    // GLOBAL VARIABLES SECTION
    /***********************************************************************************************************************************/

    private static PlayerSave _playerSave = null;
    private static Level _currentLevel = null;
    private static int _currentLevelId = 0;
    private static GameObject _camera = null;
    private static string _playerSaveToLoadPath = null;
    private static string _playerNameToSet = null;

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

    // On Awake (open game) 
    private void Awake()
    {
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
                PlayerRef.AddComponent<GameEntityObject>().Name = _playerSave.Name;
            }
        }

        // nacte pozadovany aktualni level
        LoadLevel(_playerSave.Level);
    }

    // On Level Finished
    public void OnLevelFinished()
    {
        if (PlayerRef != null)
        {
            PlayerRef.SetActive(false);
        }
    }

    // On Level Close
    public void OnLevelClose()
    {
        if (PlayerRef != null)
        {
            PlayerRef.SetActive(false);
        }
    }

    // On Level Reset
    public void OnLevelReset()
    {
        if (PlayerRef != null)
        {
            PlayerRef.SetActive(false);
        }
    }

    // On Player Death
    public void OnPlayerDeath()
    {

    }

    /***********************************************************************************************************************************/
    // GAME MANAGER UTILS
    /***********************************************************************************************************************************/

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
    /// Ukonci level. Deaktivuje hrace, zobrazi ukoncovaci prechodovy efekt, odstrani levelu.
    /// </summary>
    private void QuitLevel()
    {

    }

    /// <summary>
    /// Resetuje hrace
    /// </summary>
    private void ResetPlayer()
    {
        if (PlayerRef != null)
        {
            GameEntityObject entity = PlayerRef.AddComponent<GameEntityObject>();
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
            GameEntityObject entity = PlayerRef.AddComponent<GameEntityObject>();
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
            GameEntityObject entity = PlayerRef.AddComponent<GameEntityObject>();
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
    /// Vytvori novy save/hru
    /// </summary>
    private void CreateNewSave()
    {
        GameEntityObject entity = PlayerRef.AddComponent<GameEntityObject>();
        _playerSave = new PlayerSave
        {
            Name = _playerNameToSet == null ? "Doggy Man" : _playerNameToSet,
            Level = 1,
            PlayerRef = entity
        };
        entity.Name = _playerSave.Name;
        SaveSystem.SavePlayer(_playerSave);
        Debug.LogError("New save created");
    }

    /// <summary>
    /// Nacte save/hru ze souboru. Pokud se nacteni nepodari je automaticky vytvoren a nastaven novy save.
    /// </summary>
    private void LoadPlayerSave()
    {
        Debug.Log("Loading player save: " + _playerSaveToLoadPath);
        _playerSave = SaveSystem.LoadPlayer(_playerSaveToLoadPath);
        if (_playerSave != null)
        {
            GameEntityObject entity = PlayerRef.AddComponent<GameEntityObject>();
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
                    _currentLevelId = levelId;
                    Debug.Log("Level loading done");
                    // pripravi hrace
                    ResetPlayer();
                    SpawnPlayer();
                    ActivatePlayer();
                    if (PlayerRef != null)
                    {
                        PlayerRef.SetActive(true);
                    }
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
    /// Odstrani aktualni level, ktery je nacten ve scene.
    /// </summary>
    private void UnloadCurrentLevel()
    {
        Debug.Log("Start unloading scene with ID: " + _currentLevelId);
        Scene sceneToUnload = SceneManager.GetSceneByBuildIndex(_currentLevelId);
        if (sceneToUnload.isLoaded)
        {
            StartCoroutine(UnloadSceneAsync(sceneToUnload));
        }
    }

    private IEnumerator UnloadSceneAsync(Scene scene)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        Debug.Log("Current level unloaded");
    }

    /// <summary>
    /// Spawne hrace na spawnpoint v aktualne nactenem levelu.
    /// </summary>
    private void SpawnPlayer()
    {

    }

    /// <summary>
    /// Ziska info o aktualnim levelu
    /// </summary>
    /// <returns>Deskriptor aktualniho levelu</returns>
    private Level GetCurrentLevelInfo()
    {
        return null;
    }

    /// <summary>
    /// Callback pro stav kdy je dokoncen level
    /// </summary>
    private void LevelFinishedCallback()
    {

    }

    /// <summary>
    /// Callback pro stav kdy je hra mrtev
    /// </summary>
    private void PlayerDeathCallback()
    {

    }

}

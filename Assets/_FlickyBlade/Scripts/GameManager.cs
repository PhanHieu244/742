using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using SgLib;
using System;

public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static event System.Action<TouchState> TouchStateChange;
    public static event System.Action GameModeChanged;
    public static Action StartRecord;

    public static GameManager Instance { get; private set; }

    public static int gameMode = 1;
    
    public static event System.Action<GameState, GameState> GameStateChanged;
    public static event System.Action<GameObject> NewKnifeHasBeenSpawned;
    public static event System.Action<int> GameModeIsGoingToChange;

    private static bool isRestart;

    public enum TouchState
    {
        Enter,
        Move,
        Exit
    }

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                if (GameStateChanged != null)
                    GameStateChanged(_gameState, oldState);
            }
        }
    }

    private void ChangeLayer(GameObject knife)
    {
        knife.layer = 10;
        if (knife.transform.childCount>0)
        {
            List<GameObject> child = new List<GameObject>();
            for (int i = 0; i < knife.transform.childCount; i++)
            {
                child.Add(knife.transform.GetChild(i).gameObject);
            }
            foreach (var item in child)
            {
                ChangeLayer(item);
            }
        }
    }

    public static int GameCount
    { 
        get { return _gameCount; } 
        private set { _gameCount = value; } 
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = 30;

    [Header("Current game state")]
    [SerializeField]
    private GameState _gameState = GameState.Prepare;

    // List of public variable for gameplay tweaking
    [Header("Gameplay Config")]

    [Range(0f, 1f)]
    public float coinFrequency = 0.1f;
    public Vector3 knifeDefaultPosition = new Vector3(0, 0, 0);
    public float dragForce = 3.2f;
    public float torque = 7f;
    public float minimumDragForce = 6f;
    [Range(0, Mathf.PI)]
    public float hideKnifeTextureAngle = Mathf.PI / 2;
    public float movingTargetSpeed = 5f;
    public bool shockWaveEffect = false;
    // List of public variables referencing other objects
    [Header("Object References")]
    public PlayerController playerController;
    public GameObject touch1, touch2;
    public List<GameObject> touchChildList = new List<GameObject>();
    public List<GameObject> gameModeList = new List<GameObject>();
    public GameObject knife;
    public GameObject dirtParticlePrefab = null;
    void OnEnable()
    {
        PlayerController.PlayerDied += PlayerController_PlayerDied;
        PlayerController.RespawnKnifeEvent += OnRespawnKnife;
        MovingTarget.RespawnKnife += OnRespawnKnife;
        GameStateChanged += OnGameStateChanged;
        PlayerController.KnifeFallOutEvent += OnKnifeFallOut;
        PlayerController.KnifeStuck += OnKnifeStuck;
    }

    void OnDisable()
    {
        PlayerController.PlayerDied -= PlayerController_PlayerDied;
        PlayerController.RespawnKnifeEvent -= OnRespawnKnife;
        MovingTarget.RespawnKnife -= OnRespawnKnife;
        GameStateChanged -= OnGameStateChanged;
        PlayerController.KnifeFallOutEvent -= OnKnifeFallOut;
        PlayerController.KnifeStuck -= OnKnifeStuck;
    }

    private void OnKnifeFallOut()
    {
        Camera.main.GetComponent<CameraController>().ShakeCameraHorizontal();
    }

    private void OnGameStateChanged(GameState arg1, GameState arg2)
    {
        if (arg1.Equals(GameState.Playing))
        {
            knife = CharacterManager.Instance.characters[CharacterManager.Instance.CurrentCharacterIndex];

            PlayerController player = knife.GetComponentInChildren<PlayerController>(true);
            player.enabled = true;
            KnifeHead knifeHead = knife.GetComponentInChildren<KnifeHead>(true);
            knifeHead.enabled = true;
        }
    }

    private void OnKnifeStuck(GameObject arg1, GameObject arg2)
    {
        if (dirtParticlePrefab != null)
        {
            Vector3 particleAngle = dirtParticlePrefab.transform.eulerAngles;
            GameObject dirtParticleTemp = Instantiate(dirtParticlePrefab, arg2.GetComponentInChildren<PlayerController>().transform.position, Quaternion.identity) as GameObject;
            dirtParticleTemp.transform.eulerAngles = particleAngle;
        }
        if (gameMode!=2)
            Camera.main.GetComponent<CameraController>().ShakeCameraVertical();
    }

   
    private void OnRespawnKnife()
    {
        //destroy other knives to make sure that there is only one player in the scene
        GameObject[] knifes = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in knifes)
        {
            Destroy(item);
        }
        GameObject newKnife = Instantiate(knife, knifeDefaultPosition, Quaternion.identity) as GameObject;
        ChangeLayer(newKnife);
        if(NewKnifeHasBeenSpawned!=null)
            NewKnifeHasBeenSpawned(newKnife);
    }

    public void QuickChangeState()
    {
        StartCoroutine(QuickChangeState_IEnum());
    }

    IEnumerator QuickChangeState_IEnum()
    {
        GameOver();
        yield return new WaitForSeconds(0.2f);
        GameState = GameState.Playing;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Initial setup
        Application.targetFrameRate = targetFrameRate;
        ScoreManager.Instance.Reset();

        PrepareGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.Equals(GameState.Playing))
        {
            CheckTouchState();
        }
    }

    // Listens to the event when player dies and call GameOver
    void PlayerController_PlayerDied()
    {
        GameOver();
    }

    // Make initial setup and preparations before the game can be played
    public void PrepareGame()
    {
        GameState = GameState.Prepare;

        // Automatically start the game if this is a restart.
        if (isRestart)
        {
            isRestart = false;
            if (StartRecord != null)
                StartRecord();
            StartGame();
        }
    }

    // A new game official starts
    public void StartGame()
    {
        GameState = GameState.Playing;
        gameMode = gameModeList.Count - 1;
        ChangeGameMode();
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
        }
    }

    // Called when the player died
    public void GameOver()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
        GameState = GameState.GameOver;
        GameCount++;

        // Add other game over actions here if necessary
    }

    // Start a new game
    public void RestartGame(float delay = 0)
    {
        isRestart = true;
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HidePlayer()
    {
        if (playerController != null)
            playerController.gameObject.SetActive(false);
    }

    public void ShowPlayer()
    {
        if (playerController != null)
            playerController.gameObject.SetActive(true);
    }

    
    /*-------------------------------------------*/

    Vector3 lastRayPos = Vector3.zero;
    [HideInInspector]
    public Vector3 deltaMousePosition = Vector3.zero;
    bool touching = false;
    private void CheckTouchState()
    {
        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (touching)
                {
                    if (lastRayPos != Vector3.zero && hit.point != lastRayPos)
                    {
                        deltaMousePosition = hit.point - lastRayPos;
                        lastRayPos = hit.point;
                        if (TouchStateChange != null)
                            TouchStateChange(TouchState.Move);
                        SetTouch2Position(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }
                }
                else
                {
                    lastRayPos = hit.point;
                    if (TouchStateChange != null)
                        TouchStateChange(TouchState.Enter);
                    SetTouch1Position(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                touching = true;
            }
        }
        else
        {
            if (touching)
            {
                touching = false;
                if (TouchStateChange != null)
                    TouchStateChange(TouchState.Exit);
                ResetTouchVisualization();
            }
        }
    }

    //Touch visualization
    public void SetTouch1Position(Vector3 pos)
    {
        touch1.SetActive(true);
        Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);
        touch1.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        touch1.GetComponent<RectTransform>().anchorMax = viewPortPoint;
    }
    public void SetTouch2Position(Vector3 pos)
    {
        touch2.SetActive(true);
        
        Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);
        touch2.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        touch2.GetComponent<RectTransform>().anchorMax = viewPortPoint;
        for (int i = 0; i < touchChildList.Count; i++)
        {
            touchChildList[i].SetActive(true);
            touchChildList[i].GetComponent<RectTransform>().anchorMin = Vector2.Lerp(touch1.GetComponent<RectTransform>().anchorMin, touch2.GetComponent<RectTransform>().anchorMin, ((float) (i + 1)) / (touchChildList.Count + 1));
            touchChildList[i].GetComponent<RectTransform>().anchorMax = Vector2.Lerp(touch1.GetComponent<RectTransform>().anchorMax, touch2.GetComponent<RectTransform>().anchorMax, ((float) (i + 1)) / (touchChildList.Count + 1));
        }
    }
    public void ResetTouchVisualization() {
        touch1.SetActive(false);
        touch2.SetActive(false);
        for (int i = 0; i < touchChildList.Count; i++)
        {
            touchChildList[i].SetActive(false);
        }
    }

    //Change game scene
    public void ChangeGameMode()
    {
        ScoreManager.Instance.Reset();//reset score
        //destroy all players
        GameObject[] knifes = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in knifes)
        {
            Destroy(item);
        }
        //ChangeGameMode
        
        gameMode = (gameMode + 1) % gameModeList.Count;
        GameModeIsGoingToChange(gameMode);
        foreach (var item in gameModeList)
        {
            item.SetActive(false);
        }
        gameModeList[gameMode].SetActive(true);
        
        OnRespawnKnife();
        if(GameModeChanged!=null)
            GameModeChanged();

        if (StartRecord != null)
            StartRecord();
    }
    public void GetBackToPreviousGameMode()
    {
        ScoreManager.Instance.Reset();//reset score
        //destroy all players
        GameObject[] knifes = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in knifes)
        {
            Destroy(item);
        }
        //ChangeGameMode

        gameMode--;
        if (gameMode < 0)
            gameMode = gameModeList.Count - 1;
        GameModeIsGoingToChange(gameMode);
        foreach (var item in gameModeList)
        {
            item.SetActive(false);
        }
        gameModeList[gameMode].SetActive(true);

        OnRespawnKnife();
        if (GameModeChanged != null)
            GameModeChanged();

        if (StartRecord != null)
            StartRecord();
    }
    public void GoToMainScreen()
    {
        gameMode = gameModeList.Count-1;
        ChangeGameMode();
        GameState = GameState.Prepare;
        ResetTouchVisualization();
    }
}

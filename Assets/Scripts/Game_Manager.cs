using UnityEngine;

public enum GameState
{
    Menu, SelectionMenu, HowToPlay, Settings, Playing, Stats, Result
}

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;

    public GameState CurrentState { get; private set; }

    [Header("Managers")]
    public UI_Manager uiManager;
    public Word_Manager wordManager;
    public Stats_Manager statsManager;
    public Tile_Grid tileGrid;
    public Keyboard_Controller keyboardController;
    public Input_Handler inputHandler;

    
    private bool _lastGameWon;
    private int _lastAttemptsUsed;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        uiManager.UpdateUI(newState);
    }

    
    public void StartGame(string category, string difficulty)
    {
        // 1.
        wordManager.SelectWord(category, difficulty);

        // 2.
        uiManager.UpdateUI(GameState.Playing);

        // 3.
        tileGrid.BuildGrid(wordManager.SecretWord);

        // 4. 
        keyboardController.BuildKeyboard(tileGrid);

        // 5. 
        inputHandler.Activate(tileGrid);

        // 6. 
        uiManager.SetCategoryLabel(category);
    }

    
    public void OnVictory(int attemptsUsed)
    {
        _lastGameWon = true;
        _lastAttemptsUsed = attemptsUsed;
        statsManager.RegisterResult(true);
        inputHandler.Deactivate();
        ChangeState(GameState.Stats);
    }

    
    public void OnDefeat(int attemptsUsed)
    {
        _lastGameWon = false;
        _lastAttemptsUsed = attemptsUsed;
        statsManager.RegisterResult(false);
        inputHandler.Deactivate();
        ChangeState(GameState.Stats);
    }

    
    public void ProceedToResult()
    {
        ChangeState(GameState.Result);
    }

    
    public bool LastGameWon => _lastGameWon;
    public int LastAttemptsUsed => _lastAttemptsUsed;
    public string LastSecretWord => wordManager.SecretWord;
}